using System.Web;
using Distracey.Common.Session.OperationCorrelation;

namespace Distracey.Agent.SystemWeb.Session.OperationCorrelation
{
    public class HttpContextOperationStackStorage : IOperationStackStorage
    {
        private const string OperationStackSlot = "Distracey.OperationStack";

        public OperationStackItem Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }
                return HttpContext.Current.Items[OperationStackSlot] as OperationStackItem;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[OperationStackSlot] = value;
                }
            }
        }

        public void Clear()
        {
            HttpContext.Current.Items.Remove(OperationStackSlot);
        }
    }
}
