using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MySqlX.XDevAPI;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure._3Party
{
    public class UbuntuNginxWebConfig : IWebServerConfig, IDisposable
    {
        SshClient _sshClient;
        readonly string _ssHost;
        readonly string _sshUserName;
        readonly string _sshPassword;
        readonly IConfiguration _config;
        private IHostingEnvironment _env;
        readonly string _webFolderPath;

        public UbuntuNginxWebConfig(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
            _ssHost = _config["WebServer:Host"];
            _sshUserName = _config["WebServer:UserName"];
            _sshPassword = _config["WebServer:Password"];
            _webFolderPath = _config["WebServer:WebFolder:baza"];
            _sshClient = new SshClient(_ssHost, _sshUserName, _sshPassword);
            _sshClient.Connect();
        }
        public void AddDomain(string domain)
        {
            if (_sshClient.IsConnected)
            {
                var fileTemplatePath = Path.Combine(_env.ContentRootPath, @"FileTemplate\" + "webconfignginx");
                using (FileStream fs = System.IO.File.Create(fileTemplatePath))
                {
                    var fileContent = "server {\r\n" +
                                      "      listen 80;\r\n" +
                                      "      listen [::]:80;\r\n\r\n" +
                                      $"      root /var/www/{_webFolderPath};\r\n" +
                                      "      index index.html index.htm index.nginx-debian.html;\r\n\r\n" +
                                      $"      server_name {domain};\r\n" +
                                      "      location ~ ^/index\\.html$ { }\r\n" +
                                      "      location / {\r\n" +
                                      "          try_files $uri $uri/ =404;\r\n" +
                                      "          if (!-e $request_filename){\r\n" +
                                      "              rewrite ^(.*)$ /index.html break;\r\n" +
                                      "          }\r\n" +
                                      "      }\r\n" +
                                      "}\r\n";
                    byte[] content = new UTF8Encoding(true).GetBytes(fileContent);

                    fs.Write(content, 0, content.Length);
                    Console.WriteLine("Make file config success!");

                }

                using (SftpClient sftp = new SftpClient(_ssHost, _sshUserName, _sshPassword))
                {
                    try
                    {
                        sftp.Connect();
                        using (var fs = System.IO.File.OpenRead(fileTemplatePath))
                        {
                            var remoteFilePath = $"/etc/nginx/sites-available/{domain}";
                            sftp.UploadFile(fs, remoteFilePath, true);//, UploadCallBack);
                            Console.WriteLine("upload file config success");
                        }
                    }
                    catch (Exception ex)
                    {
                        sftp.Disconnect();
                        throw new Exception($"{ex.Message} {ex.InnerException}");
                    }
                    finally
                    {
                        sftp.Disconnect();
                    }

                }

                // Perform your SSH operations

                _sshClient.RunCommand($"sudo ln -s /etc/nginx/sites-available/{domain}  /etc/nginx/sites-enabled/{domain}");

                _sshClient.RunCommand($"sudo nginx -t");

                //Tạm thời chưa restart nginx server vì ứng dụng sẽ mất kết nối
                //_sshClient.RunCommand($"sudo systemctl restart nginx");

                Console.WriteLine("config nginx completed!");
            }
        }

        public void RemoveDomain(string domain)
        {
            if (_sshClient.IsConnected)
            {
                _sshClient.RunCommand($"sudo rm -f /etc/nginx/sites-enabled/{domain}");
                _sshClient.RunCommand($"sudo rm -f /etc/nginx/sites-available/{domain}");
                //Tạm thời chưa restart nginx server vì ứng dụng sẽ mất kết nối
                //_sshClient.RunCommand($"sudo systemctl restart nginx");
            }
        }

        public void RestartNginxServer()
        {
            if (_sshClient.IsConnected)
            {
                _sshClient.RunCommand($"sudo systemctl restart nginx");
            }
        }


        void AuthenticateSSHWithPasswordKey(SshClient sshClient)
        {
            var sshKeyFile = Path.Combine(_env.ContentRootPath, @"FileTemplate\" + "opensshkey");
            var privateKeyFile = new PrivateKeyFile(sshKeyFile);
            var connectionInfo = new ConnectionInfo(_ssHost,
                                        "nvmanh",
                                        new PasswordAuthenticationMethod("nvmanh", ""),
                                        new PrivateKeyAuthenticationMethod(sshKeyFile));

            //using (var client = new SftpClient(connectionInfo))
            //{
            //    client.Connect();
            //}
            using var sshClient2 = new SshClient(connectionInfo);


            // Load private key from file or in-memory
            sshClient2.Connect();

            // Authenticate with the server using public key
            if (sshClient2.IsConnected)
            {
                Console.WriteLine("SSH Connected!");
                // Authenticate with private key
                //sshClient.AddIdentityFile(privateKeyFile);

                // Perform your SSH operations
            }
            else
            {
                Console.WriteLine("SSH Not Connected!");
            }
        }

        public void Dispose()
        {
            _sshClient.Disconnect();
            if (!_sshClient.IsConnected)
            {
                // Perform your SSH operations
                Console.WriteLine("SSH Disconnected!");
            }
            _sshClient.Dispose();
        }
    }
}
