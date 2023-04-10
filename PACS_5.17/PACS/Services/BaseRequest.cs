using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Service
{
    public class BaseRequest
    {
        public Method Method { get; set; }
        public string Route { get; set; }
        public string ContentType { get; set; } = "application/json";
        public List<string> FilePath { get; set; }
        public object Parameter { get; set; }

        //用于自动诊断的图片
        public byte[] AutoDiagnoseImage { get; set; }
        //自动诊断热力图
        public byte[] ThermodynamicChart { get; set; }
        //自动诊断标注图
        public byte[] LabelImage { get; set; }
        
    }
}
