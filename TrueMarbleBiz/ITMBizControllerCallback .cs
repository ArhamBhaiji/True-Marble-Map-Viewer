using System.ServiceModel;

namespace TrueMarbleBiz
{
    [ServiceContract]
    public interface ITMBizControllerCallback
    {
        [OperationContract]
        void OnVerificationComplete(bool check);
    }
}
