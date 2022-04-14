import http from "k6/http";
import { group, check, sleep } from "k6";

const BASE_URL = "/";
// Sleep duration between successive requests.
// You might want to edit the value of this variable or remove calls to the sleep function on the script.
const SLEEP_DURATION = 0.1;
// Global variables should be initialized.

export default function() {
    group("/Chat/message/{messageId}", () => {
        let messageId = 'TODO_EDIT_THE_MESSAGEID'; // specify value as there is no example value for this parameter in OpenAPI spec

        // Request No. 1
        {
            let url = BASE_URL + `/Chat/message/${messageId}`;
            // TODO: edit the parameters of the request body.
            let body = {"message": "string"};
            let params = {headers: {"Content-Type": "application/json", "Accept": "application/json"}};
            let request = http.put(url, JSON.stringify(body), params);

            check(request, {
                "Successfully updated": (r) => r.status === 204
            });

            sleep(SLEEP_DURATION);
        }

        // Request No. 2
        {
            let url = BASE_URL + `/Chat/message/${messageId}`;
            let request = http.del(url);

            check(request, {
                "Success": (r) => r.status === 200
            });
            check(request, {
                "Message successfully deleted": (r) => r.status === 204
            });
        }
    });

    group("/Chat", () => {

        // Request No. 1
        {
            let url = BASE_URL + `/Chat`;
            let params = {headers: {"Content-Type": "application/json", "Accept": "application/json"}};
            let request = http.post(url, params);

            check(request, {
                "Connection successfully created": (r) => r.status === 200
            });
        }
    });

    group("/Chat/message", () => {

        // Request No. 1
        {
            let url = BASE_URL + `/Chat/message`;
            // TODO: edit the parameters of the request body.
            let body = {"chatId": "uuid", "senderId": "uuid", "message": "string"};
            let params = {headers: {"Content-Type": "application/json", "Accept": "application/json"}};
            let request = http.post(url, JSON.stringify(body), params);

            check(request, {
                "": (r) => r.status === 200
            });
        }
    });

    group("/Chat/search/{userId}", () => {
        let take = 'TODO_EDIT_THE_TAKE'; // specify value as there is no example value for this parameter in OpenAPI spec
        let skip = 'TODO_EDIT_THE_SKIP'; // specify value as there is no example value for this parameter in OpenAPI spec
        let userId = 'TODO_EDIT_THE_USERID'; // specify value as there is no example value for this parameter in OpenAPI spec

        // Request No. 1
        {
            let url = BASE_URL + `/Chat/search/${userId}?skip=${skip}&take=${take}`;
            let request = http.get(url);

            check(request, {
                "User chat ids": (r) => r.status === 200
            });
        }
    });

    group("/Chat/{chatId}/history", () => {
        let take = 'TODO_EDIT_THE_TAKE'; // specify value as there is no example value for this parameter in OpenAPI spec
        let chatId = 'TODO_EDIT_THE_CHATID'; // specify value as there is no example value for this parameter in OpenAPI spec
        let skip = 'TODO_EDIT_THE_SKIP'; // specify value as there is no example value for this parameter in OpenAPI spec

        // Request No. 1
        {
            let url = BASE_URL + `/Chat/${chatId}/history?skip=${skip}&take=${take}`;
            let request = http.get(url);

            check(request, {
                "Chat history": (r) => r.status === 200
            });
        }
    });

    group("/Chat/{chatId}", () => {
        let chatId = 'TODO_EDIT_THE_CHATID'; // specify value as there is no example value for this parameter in OpenAPI spec

        // Request No. 1
        {
            let url = BASE_URL + `/Chat/${chatId}`;
            let request = http.get(url);

            check(request, {
                "Success": (r) => r.status === 200
            });

            sleep(SLEEP_DURATION);
        }

        // Request No. 2
        {
            let url = BASE_URL + `/Chat/${chatId}`;
            let request = http.del(url);

            check(request, {
                "Success": (r) => r.status === 200
            });
            check(request, {
                "Chat successfully deleted": (r) => r.status === 204
            });
        }
    });

}
