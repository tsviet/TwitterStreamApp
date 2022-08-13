namespace TwitterStreamV2App.Interfaces;

public interface IQueueService
{
    public void SendMessage(object message);
}