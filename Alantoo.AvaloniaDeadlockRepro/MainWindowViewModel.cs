using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using ReactiveUI;

namespace Alantoo.AvaloniaDeadlockRepro
{
    [DataContract]
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Courses = new ObservableCollection<CourseViewModel>
            {
                new CourseViewModel()
            };

            var worker = new BackgroundWorker();
            worker.DoWork += WorkerOnDoWork;
            worker.RunWorkerAsync();
        }


        private void WorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var course = Courses[0];
            
            course.Status = CourseDownloadStatus.InProgress;
            Thread.Sleep(500);
            course.Status = CourseDownloadStatus.ReadyToStart;
            Thread.Sleep(500);
            course.Status = CourseDownloadStatus.InProgress;
            Thread.Sleep(500);
            course.Status = CourseDownloadStatus.ReadyToStart;
            // while (true)
            // {
            //     course.Status = course.Status == CourseDownloadStatus.InProgress
            //         ? CourseDownloadStatus.ReadyToStart
            //         : CourseDownloadStatus.InProgress;
            //     Thread.Sleep(500);
            // }
        }

        public IList<CourseViewModel> Courses { get; set; }
    }
    
    public class CourseViewModel : ReactiveObject
    {
        private CourseDownloadStatus _status;
        private bool _areDownloadingNotStarted;

        public CourseViewModel()
        {
            this.WhenAnyValue(_ => _.Status)
                .Select(status => status != CourseDownloadStatus.InProgress)
                .ObserveOn(RxApp.MainThreadScheduler)
                // .ToProperty(this, _ => _.AreDownloadingNotStarted);
                .BindTo(this, _ => _.AreDownloadingNotStarted);

            PropertyChanged += (sender, args) => Console.WriteLine("Changed " + args.PropertyName);
        }
        
        public bool AreDownloadingNotStarted
        {
            get => _areDownloadingNotStarted;
            set => this.RaiseAndSetIfChanged(ref _areDownloadingNotStarted, value);
        }

        public CourseDownloadStatus Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
    }
    
    public enum CourseDownloadStatus
    {
        ReadyToStart,
        InProgress
    }
}