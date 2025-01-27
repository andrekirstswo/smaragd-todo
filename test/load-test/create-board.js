import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
    iterations: 10
};

export default function() {
    const name = generateText();
    const json = JSON.stringify({ name: name })
    const params = {
        headers: {
            'Context-Type': 'application/json'
        }
    }
    http.post('http://localhost:7270/api/board', json, params);

    sleep(1);
}


function generateText() {
    const length = 16;
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
      counter += 1;
    }
    return result;
}