import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { MessageModel } from './MessageModel';

@Injectable({
  providedIn: 'root'
})

export class SignalRService {
  public message: MessageModel[] = [];
  public connectionId = '';
  public broadcastData: MessageModel[] = [];
  public senderId = '3fa85f64-5717-4562-b3fc-2c963f66afa1';
  public receiverId = '3fa85f64-5717-4562-b3fc-2c963f66afa2';
  public chatId = '3fa85f64-5717-4562-b3fc-2c963f66afa6';

  private hubConnection: signalR.HubConnection

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/chat')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Started'))
      .then(() => this.getConnectionId())
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  public sendMessage = () => {

  }

  public getConnectionId(): string {
    return '';
}

  public getExistConnection = () => {
    this.hubConnection.invoke('getconnectionid').then(
      (data) => {
        console.log(data);
        this.connectionId = data;
      }
    )
  }

  public broadcastChartData = () => {
    this.hubConnection.invoke('message', this.message, this.connectionId)
      .catch(err => console.error(err));
  }

  public addBroadcastChartDataListener = () => {
    this.hubConnection.on('message', (data) => {
      this.broadcastData = data;
    })
  }
}
