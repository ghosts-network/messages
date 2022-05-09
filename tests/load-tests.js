import http from 'k6/http';

export const options = {
    vus: 10,
    duration: '30s',
};

export default function () {
    const url = 'http://localhost:5252/chats/';
    const payload = JSON.stringify({
        name: 'test',
        participants: ['3fa85f64-5717-4562-b3fc-2c963f66afa6', '3fa85f64-5717-4562-b3fc-2c963f66afa7'],
    });
    const params = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    http.post(url, payload, params);
}
