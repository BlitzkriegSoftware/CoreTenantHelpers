using System.Collections.Generic;
using System.Linq;

namespace BlitzkriegSoftware.Tenant.Demo.Web.Models
{
    /// <summary>
    /// Error Payload
    /// </summary>
    public class ErrorPayload
    {
        /// <summary>
        /// Gets or sets hTTP Status Code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets stack Trace
        /// </summary>
        public string StackTrace { get; set; }

        private Dictionary<string, string> _data = null;

        /// <summary>
        /// Gets or sets additional Data
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public Dictionary<string, string> Data { get
            {
                if (this._data == null) this._data = new Dictionary<string, string>();
                return this._data;
            }
            set {
                this._data = value;
            } 
        }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Nice Debugging String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{this.StatusCode}] {this.Message}\n{this.StackTrace}\n{ string.Join(";", this.Data.Select(x => x.Key + "=" + x.Value).ToArray())}";
        }
    }
}
