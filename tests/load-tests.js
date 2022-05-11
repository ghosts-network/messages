import http from 'k6/http';

const baseUrl = 'http://localhost:5252';
const messagesPerChat = 20;

export const options = {
    vus: 50,
    duration: '300s',
};

export default function () {
    const chat = createChat();
    for (let i = 0; i < messagesPerChat; i++) {
        sendMessage(chat.id, 'Hello, world!');
        getMessages(chat.id);
    }
}

function sendMessage(chatId, content) {
    const url = `${baseUrl}/chats/${chatId}/messages`;
    const payload = JSON.stringify({
        content: content,
        senderId: '3fa85f64-5717-4562-b3fc-2c963f66afa6'
    });
    const params = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    http.post(url, payload, params);
}

function getMessages(chatId) {
    const url = `${baseUrl}/chats/${chatId}/messages`;
    
    return http.get(url);
}

function createChat() {
    const url = `${baseUrl}/chats`;
    const payload = JSON.stringify({
        name: 'test',
        participants: ['3fa85f64-5717-4562-b3fc-2c963f66afa6', '3fa85f64-5717-4562-b3fc-2c963f66afa7'],
    });
    const params = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    const r = http.post(url, payload, params);
    return JSON.parse(r.body);
}
