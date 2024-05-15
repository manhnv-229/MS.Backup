/* eslint-disable */
function addStyles(win, styles) {
    // styles.forEach((style) => {
    //     let link = win.document.createElement("link");
    //     link.setAttribute("rel", "stylesheet");
    //     link.setAttribute("type", "text/css");
    //     link.setAttribute("href", style);
    //     win.document.getElementsByTagName("head")[0].appendChild(link);
    // });
    var newStyles = document.querySelectorAll('link');
    console.log('newStyles: ', newStyles);
    for (const link of newStyles) {
        win.document.getElementsByTagName("head")[0].appendChild(link);
    }
}

function getStyles() {
    // Lấy toàn bộ style:
    // Get all stylesheets HTML
    let stylesHtml = '';

    for (const node of[...document.querySelectorAll('style[type="text/css"], style')]) {
        stylesHtml += node.outerHTML;
    }
    console.log('stylesHtml: ', stylesHtml);
    return stylesHtml;
}

const print = {
    install(app, options = {}) {
        app.config.globalProperties.$htmlToPaper = (
            el,
            localOptions,
            cb = () => true
        ) => {
            let defaultName = "_blank",
                defaultSpecs = ["fullscreen=yes", "titlebar=yes", "scrollbars=yes"],
                defaultReplace = true,
                defaultStyles = [];
            let {
                name = defaultName,
                    specs = defaultSpecs,
                    replace = defaultReplace,
                    styles = defaultStyles
            } = options;
            console.log(`localOptions`, localOptions);
            // If has localOptions
            // TODO: improve logic
            if (!!localOptions) {
                if (localOptions.name) name = localOptions.name;
                if (localOptions.specs) specs = localOptions.specs;
                if (localOptions.replace) replace = localOptions.replace;
                if (localOptions.styles) styles = localOptions.styles;
            }

            specs = !!specs.length ? specs.join(",") : "";

            const element = window.document.getElementById(el);

            if (!element) {
                alert(`Element to print #${el} not found!`);
                return;
            }

            var linkHMTL = '';
            var links = document.querySelectorAll('link');
            if (links && links.length > 0) {
                for (const link of links) {
                    linkHMTL += link.outerHTML;
                }
            }
            const url = "";
            const win = window.open(url, name, specs, replace);
            const stylesCss = getStyles();
            win.document.write(`
          <html>
            <head>
              <title>NVNMANH</title>
              ${stylesCss}
              ${linkHMTL}
            </head>
            <body>
              ${element.outerHTML}
            </body>
          </html>
        `);

            // // addStyles(win, styles);
            // var isMobile = this.commonJs.mobileCheck();
            // // var isTablet = this.commonJs.mobileCheck();
            // // var isMobile = navigator.userAgentData.mobile;
            // if (isMobile) {
            //     this.hubConnection.invoke("PrintFromMobile");
            // } else {
            //     setTimeout(() => {
            //         win.document.close();
            //         win.focus();
            //         win.print();
            //         win.close();
            //         cb();
            //     }, 1000);
            // }

            setTimeout(() => {
                win.document.close();
                win.focus();
                win.print();
                win.close();
                cb();
            }, 1000);

            return true;
        };
    }
};

export default print;