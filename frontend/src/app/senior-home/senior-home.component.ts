import { Component } from '@angular/core';

interface SeniorHomeItem {
  label: string;
  icon: string;
  route: string;
  description: string;
}

@Component({
  selector: 'app-senior-home',
  templateUrl: './senior-home.component.html',
  styleUrls: ['./senior-home.component.scss']
})
export class SeniorHomeComponent {
  items: SeniorHomeItem[] = [
    {
      label: 'HOME.CHAT',
      icon: '💬',
      route: '/chat',
      description: 'HOME.CHAT_DESC'
    },
    {
      label: 'HOME.AGENDA',
      icon: '📅',
      route: '/agenda',
      description: 'HOME.AGENDA_DESC'
    },
    {
      label: 'HOME.REMINDERS',
      icon: '⏰',
      route: '/reminders',
      description: 'HOME.REMINDERS_DESC'
    },
    {
      label: 'HOME.ACTIVITIES',
      icon: '🎯',
      route: '/activities',
      description: 'HOME.ACTIVITIES_DESC'
    },
    {
      label: 'HOME.PROFILE',
      icon: '👤',
      route: '/profil',
      description: 'HOME.PROFILE_DESC'
    }
  ];
}