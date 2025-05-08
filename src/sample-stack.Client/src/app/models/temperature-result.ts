export class TemperatureResult {

  public temp: number;
  public timestamp: Date;

  public constructor() {
    this.temp = 0;
    this.timestamp = new Date();
  }
}
