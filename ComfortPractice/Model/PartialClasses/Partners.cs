using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComfortPractice.Model;

namespace ComfortPractice.Model
{
    public partial class Partners
    {
        public string SizeDiscount
        {
            get
            {
                var requests = Connection.connect.Request.Where(r => r.Id_partner == this.Id_partner && r.Id_status == 5).ToList();
                decimal totalSum = 0;
                foreach (var request in requests)
                {
                    var requestDetails = Connection.connect.RequestDetails.Where(rd => rd.Id_request == request.Id_request).ToList();
                    foreach (var rd in requestDetails)
                    {
                        var product = Connection.connect.Products.FirstOrDefault(p => p.Id_product == rd.Id_product);
                        if (product != null && product.MinPrice.HasValue && rd.Quantity.HasValue)
                        {
                            totalSum += rd.Quantity.Value * product.MinPrice.Value;
                        }
                    }
                }
                if (totalSum > 10000 && totalSum < 50000)
                    return "5%";
                else if (totalSum >= 50000 && totalSum < 300000)
                    return "10%";
                else if (totalSum >= 300000)
                    return "15%";
                else
                    return null;
            }
        }
    }
}