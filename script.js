import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  duration: '60s',
  vus: 300,
};

export default function () {
  let res = http.get('http://localhost:5024/api/TodoItems');
  check(res, { 'status 200 OK': (r) => r.status === 200 });
  sleep(1);
}