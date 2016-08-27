using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Threading;

/// <summary>
/// The following class takes in a url and raises the http request and replies back with the respomse
/// the request is raised in a thread to stop it from indefinitelu blockin the calling thread
/// </summary>
public class RequestHandler
{
    volatile string m_response;
    volatile string m_url;
    volatile bool m_error = false;
    AutoResetEvent m_e = new AutoResetEvent(false);
    Thread m_thread;
	public RequestHandler(string url)
	{
        m_url = url;
	}
    void ThreadProc()//raise request in thread
    {
        HttpWebResponse HttpWResp = null;
        Stream streamResponse = null;

        StreamReader reader = null;
        string response = null;
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(m_url);
            HttpWResp = (HttpWebResponse)request.GetResponse();
            streamResponse = HttpWResp.GetResponseStream();

            reader = new StreamReader(streamResponse);
            response = reader.ReadToEnd();
        }
        catch (WebException ex)
        {
            response = "Could not connect to server. Have you created an API key?";
            m_error = true;
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
            m_response = response;
            m_e.Set();//set the event so that waitone would quit
        }

    }
    public string Process()
    {
        //run in thread so that callinf thread shud nebver block
        m_thread= new Thread(new ThreadStart(ThreadProc));
        m_thread.Start();
        bool signalled=m_e.WaitOne(10000);
        if (!signalled)
        {

            //if event did not recieve a signalthen abort the thread
            m_thread.Abort();
            return null;
        }
        if (m_error)
            throw new ApiKeyException(m_response);
        return m_response;

    }
}
public class ApiKeyException:Exception
{
    public ApiKeyException(string msg):base(msg)
    {

    }
}