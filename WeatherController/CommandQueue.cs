using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherController
{
    public class CommandQueue
    {
        Queue<Command> m_commands = new Queue<Command>();
        private object synch = new object();
        private static object singletonsynch = new object();

        static CommandQueue m_singleobj;
        private CommandQueue()
        {
        }
        public static CommandQueue Instance()//Singleton with thread synch
        {
            if (m_singleobj != null)
                return m_singleobj;
            else
            {
                lock (singletonsynch)//Locking singleton
                {
                    if (m_singleobj == null)
                        m_singleobj = new CommandQueue();
                    return m_singleobj;
                }

            }
        }
        public void add(Command cmd)//adds a command item to queue
        {
            lock (synch)
            {
                m_commands.Enqueue(cmd);
            }
            //once we add a command, the commanmanager could be sleeping. we need to wakeit up and ask it to execute it
            CommandExecutor.Instance().Initiate();

        }
        public Command get()
        {
            Command cmd =null;
            lock (synch)
            {
                cmd= m_commands.Dequeue();
            }
            return cmd;
        }
        public bool empty()
        {
            lock (synch)
            {
                if (m_commands.Count == 0)
                    return true;
                else
                    return false;
            }
        }

    }
}
