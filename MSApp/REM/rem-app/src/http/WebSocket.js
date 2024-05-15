const NOTICE_URL = (VUE_APP_NOTICE_URL ? VUE_APP_NOTICE_URL : "/");

import { HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr'

export default {
    createHub() {
        return new HubConnectionBuilder()
            .withUrl(NOTICE_URL + '/NotificationHub', {
                accessTokenFactory: () => localStorage.getItem("user-token"),
                skipNegotiation: true,
                // headers: { "clientHost": location.host },
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .configureLogging(LogLevel.None)
            .build();
    }
}