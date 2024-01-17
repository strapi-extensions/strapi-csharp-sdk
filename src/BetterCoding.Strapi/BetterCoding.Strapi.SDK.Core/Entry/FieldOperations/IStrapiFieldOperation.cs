using BetterCoding.Strapi.SDK.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCoding.Strapi.SDK.Core.Entry.FieldOperations
{
    public interface IStrapiFieldOperation
    {
        object Encode(IServiceHub serviceHub);

        IStrapiFieldOperation MergeWithPrevious(IStrapiFieldOperation previous);

        object Apply(object oldValue, string key);
    }
}
