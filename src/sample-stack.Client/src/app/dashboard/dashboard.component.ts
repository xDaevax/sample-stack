import { Component, OnDestroy, OnInit } from '@angular/core';
import { TemperatureService } from '../shared/services/temperature.service';
import { BehaviorSubject } from 'rxjs';
import { TemperatureResult } from '../models/temperature-result';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [AsyncPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {

  private timer: ReturnType<typeof setInterval> | null;
  protected temp: BehaviorSubject<TemperatureResult>;

  public constructor(private temperatureService: TemperatureService) {
    this.timer = null;
    this.temp = new BehaviorSubject<TemperatureResult>(new TemperatureResult());
  }

  public ngOnInit(): void {
    this.timer = setInterval(() => {
      this.temperatureService.currentTemperature().subscribe(result => {
        this.temp.next(result);
      })
    }, 1000);
  }

  public ngOnDestroy(): void {
    clearInterval(this.timer as ReturnType<typeof setInterval>);
  }
}
