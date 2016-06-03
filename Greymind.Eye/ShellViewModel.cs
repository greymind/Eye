using System;
using System.Threading;
using System.Windows;

namespace Greymind.Eye
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private const int SnoozeIntervalInMinutes = 5;
        private const string SnoozeState = "Snooze";

        private Timer timer;

        private bool isEnabled;
        private string interval;

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                NotifyOfPropertyChange(nameof(IsEnabled));

                if (IsEnabled) StartTimerWithUserInterval();
                else StopTimer();
            }
        }

        public string Interval
        {
            get { return this.interval; }
            set
            {
                this.interval = value;
                NotifyOfPropertyChange(nameof(Interval));
            }
        }

        public ShellViewModel()
        {
            Interval = "20";
        }

        private void StartTimerWithUserInterval()
        {
            int intervalInMinutes;
            if (int.TryParse(Interval, out intervalInMinutes))
            {
                StartTimer(intervalInMinutes, string.Empty);
            }
        }

        private void StartTimer(int intervalInMinutes, string state)
        {
            this.timer = new Timer(TimerElapsed, state, TimeSpan.FromMinutes(intervalInMinutes), TimeSpan.Zero);
        }

        private void StopTimer()
        {
            this.timer.Dispose();
        }

        private void TimerElapsed(object state)
        {
            var isSnooze = (string)state == SnoozeState;

            StopTimer();

            var result = MessageBox.Show
                (
                    messageBoxText: isSnooze ? "Snooze time is up!" : "Time to take a break!",
                    caption: "Greymind Eye",
                    button: MessageBoxButton.OKCancel,
                    icon: MessageBoxImage.Information,
                    defaultResult: MessageBoxResult.Cancel
                );

            switch (result)
            {
                case MessageBoxResult.OK:
                    StartTimerWithUserInterval();
                    break;

                case MessageBoxResult.Cancel:
                    StartSnoozeTimer();
                    break;
            }
        }

        private void StartSnoozeTimer()
        {
            StartTimer(SnoozeIntervalInMinutes, SnoozeState);
        }
    }
}