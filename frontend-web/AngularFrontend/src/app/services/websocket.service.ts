// websocket.service.ts
import { Injectable } from '@angular/core';
import { Client, Message, StompSubscription, StompConfig, StompHeaders } from '@stomp/stompjs';
import {BehaviorSubject} from 'rxjs';
import {QuestionMessage} from '../dto/QuestionMessage';
import {CheckMessage} from '../dto/CheckMessage';
import {ScoreMessage} from '../dto/ScoreMessage';
import {StartMessage} from '../dto/StartMessage';
import {AnswerMessage} from '../dto/AnswerMessage';
import {environment} from '../enviroments/enviroments';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private client: Client;
  private connectionPromise: Promise<void>;

  public questionSubject = new BehaviorSubject<any>(null);
  public checkSubject = new BehaviorSubject<any>(null);
  public scoreSubject = new BehaviorSubject<any>(null);


  constructor() {
    // create a new client
    const wsUrl: string = environment.API_BACKEND_JAVA.replace(/^http/, 'ws') + '/ws';
    this.client = new Client({
      brokerURL: wsUrl,
      reconnectDelay: 5000,
      debug: (str) => { console.log(str); },
      onStompError: (frame) => {
        console.error('STOMP error', frame.headers['message'], frame.body);
      },
    });
    // Wrap onconnect in a Promise
    this.connectionPromise = new Promise<void>((resolve) => {
      this.client.onConnect = () => {
        // init subscribes
        this.client.subscribe('/user/queue/next', (msg: Message) => {
          this.questionSubject.next(JSON.parse(msg.body) as QuestionMessage);
        });
        this.client.subscribe('/user/queue/check', (msg: Message) => {
          this.checkSubject.next(JSON.parse(msg.body) as CheckMessage);
        });
        this.client.subscribe('/user/queue/score', (msg: Message) => {
          this.scoreSubject.next(JSON.parse(msg.body) as ScoreMessage);
        });
        resolve();
      };
    });
    // activate the client
    this.client.activate();
  }

  // helper to wait until the client is connected
  async waitForConnection(): Promise<void> {
    return this.connectionPromise
  }

  // connect the client if not already is
  connect(): void {
    if (!this.client.active) {
      this.client.activate();
    }
  }

  // send template function
  send<T>(destination: string, body: T): void {
    if (this.client.connected) {
      this.client.publish({
        destination,
        body: JSON.stringify(body),
      });
    } else {
      console.error('WebSocket is not connected.');
    }
  }

  startQuiz(data: StartMessage): void {
    this.send('/app/start', data);
  }

  submitAnswer(data: AnswerMessage): void {
    this.send('/app/answer', data);
  }

  requestScore(): void {
    this.send('/app/score', {});
  }

  requestNextQuestion(): void {
    this.send('/app/next', {});
  }

  disconnect() {
    if (this.client.active) {
      this.client.deactivate();
    }
  }
}
