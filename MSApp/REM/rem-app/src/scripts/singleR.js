/* eslint-disable */
import WebSocket from "@/http/WebSocket";
import store from "@/store";
import { ADD_NEW_SLOT, REFRESH_INVENTORY_DATA } from "@/store/actions/entity";
import { SET_NEW_HUB_CONNECTION } from "@/store/actions/signalR";
import commonJs from "./common";

/** -----------------------------------------
* Kết nối với signalR
* @returns hubConnection
*/
export function createHubConnection() {
    var hubConnection = WebSocket.createHub();
    hubConnection.on("UpdateOrganizationInfo", (orgInfo) => {
        store.dispatch("UPDATE_ORGANIZATION_INFO", orgInfo);
    });

    hubConnection.on("ReceiveNotificationWhenDisconnected", (username) => {
        console.log(`${username} đã ngắt kết nối!`);
    });

    hubConnection.on("SaveConnectionId", (connectionId) => {
        console.log(`connectionId:  ${connectionId}`);
        localStorage.setItem("connectionId", connectionId);
    });

    hubConnection.on("ShowAlertWhenOnline", (username) => {
        console.log(`${username} đã kết nối!`);
    });

    hubConnection.on(
        "RecieveNotifiedWhenContactRegistedEventSuccess",
        (eventInfo, contactInfo) => {
            console.log("eventInfo:", eventInfo);
            console.log("contactInfo:", contactInfo);
        }
    );

    hubConnection.on("PrintFromMobile", (invoiceId, connectionId) => {
        var localConnectionId = localStorage.getItem("connectionId");
        console.log("PrintFromMobile");
        console.log(`localConnection: `, localConnectionId);
        console.log(`connectionId: `, connectionId);
        if (localConnectionId != connectionId) {
            router.push(`/invoices/printer/${invoiceId}/mobile`);
        }
    });

    hubConnection.on(
        "ShowPecentUpload",
        (
            currentFileUpload,
            totalFileUpload,
            isFinish,
            totalTimes,
            progressInfo
        ) => {
            // Ẩn loading nếu có:
            commonJs.hideLoading();
            progressInfo = {
                Id: progressInfo.id,
                Name: progressInfo.name,
                Value: currentFileUpload,
                Max: totalFileUpload,
                Message: `Đang tải lên ${currentFileUpload}/${totalFileUpload} ảnh.`,
            };
            if (totalTimes > 10) {
                var pecent = ((currentFileUpload / totalFileUpload) * 100).toFixed(2);
                progressInfo.RunBackground = true;
                progressInfo.Message = `Đang tạo Album: ${pecent}%.`;
            }
            commonJs.showProgress(progressInfo);
            if (isFinish) {
                setTimeout(function () {
                    commonJs.hideProgress();
                }, 500);
            }
        }
    );

    hubConnection.on(
        "ShowPecentDeleted",
        (
            indexFileDelete,
            totalFileDelete,
            isFinish,
            totalTimes,
            progressInfo
        ) => {
            // Ẩn loading nếu có:
            commonJs.hideLoading();
            progressInfo = {
                Id: progressInfo.id,
                Name: progressInfo.name,
                Value: indexFileDelete,
                Max: totalFileDelete,
                Message: `Đang xóa ${indexFileDelete}/${totalFileDelete} ảnh.`,
            };
            if (totalTimes > 10) {
                var pecent = ((indexFileDelete / totalFileDelete) * 100).toFixed(2);
                progressInfo.RunBackground = true;
                progressInfo.Message = `Đang xóa Album: ${pecent}%.`;
            }
            commonJs.showProgress(progressInfo);
            if (isFinish) {
                setTimeout(function () {
                    commonJs.hideProgress();
                }, 500);
            }
        }
    );
    hubConnection.on("RefreshInventoryItems", () => {
        store.dispatch(REFRESH_INVENTORY_DATA);
    });

    hubConnection.on("ProcessAfterAddNewSlotSuccess", (slotDto) => {
        store.dispatch(ADD_NEW_SLOT, slotDto);
    });

    if (store.getters.isAuthenticated) {
        commonJs.showConnectingHub();
        hubConnection
            .start()
            .then(() => {
                console.log("Đã kết nối tới Hub...");
                // Cập nhật store:
                store.dispatch(SET_NEW_HUB_CONNECTION, hubConnection);
                // hubConnection.invoke("GetClassInfo");
                commonJs.hideConnectingHub();
            })
            .catch((err) => {
                console.error(err);
                commonJs.hideConnectingHub();
            });
    }
    return hubConnection;
}