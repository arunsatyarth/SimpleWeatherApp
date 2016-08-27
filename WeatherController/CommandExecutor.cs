using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WeatherController
{
    public class CommandExecutor
    {
        private object queuesynch = new object();
        private static object singletonsynch = new object();
        private AutoResetEvent m_wait = new AutoResetEvent(false);
        static CommandExecutor m_singleobj;
        Thread m_thread=null;
        private CommandExecutor()
        {
            m_thread=new Thread(new ThreadStart(ThreadProc));
            m_thread.Start();

        }
        public static CommandExecutor Instance()
        {
            if (m_singleobj != null)
                return m_singleobj;
            else
            {
                lock (singletonsynch)//Locking singleton
                {
                    if(m_singleobj==null)
                        m_singleobj = new CommandExecutor();
                    return m_singleobj;
                }
                
            }
        }
        public void Close()
        {
            m_thread.Abort();
        }
        void execute()
        {
            Command command = CommandQueue.Instance().get();
            if(command!=null)
                command.Execute();
        }
        void ThreadProc()
        {
            while (true)
            {
                if (CommandQueue.Instance().empty())
                {
                    m_wait.Reset();
                    m_wait.WaitOne();

                }
                execute();
            }
            

        }
        public void Initiate()
        {
            //sets the AutoresetEvent which will wakeup the commandmanager to execute the item from commandqueue
            m_wait.Set();
        }
    }
}
