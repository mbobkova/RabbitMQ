using System;
using EasyNetQ;
using Quartz;
using Quartz.Impl;
using Messages;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
        }

        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailSender>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity("trigger1", "group1")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithIntervalInSeconds(5)
                                .RepeatForever())
                            .Build();

            scheduler.ScheduleJob(job, trigger);
        }

    }

    public class EmailSender : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                string input = "denisvkozeka@gmail.com";
                Console.WriteLine("Sent message: " + input);
                bus.Publish(new Message
                {
                    Text = input
                });
            }
        }
    }
}