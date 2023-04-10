using PACS.Shared.Entities;
using PACS.WebAPI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace PACS.WebAPI.Services
{
    public class AutoLableService : IAutoLableService
    {
        private readonly IdentityContext identityContext;

        public AutoLableService(IdentityContext identityContext)
        {
            this.identityContext = identityContext;
        }

        public async Task<bool> ClassifyAsync(string FileId)
        {
            var item = identityContext.FileItems.Single(i => i.FileItemId.Equals(FileId));

            using (MemoryStream ms = new MemoryStream(item.Image))
            {
                Image outputImg = Image.Load(ms);
                outputImg.Save(item.Name);
                ;
            }


            /*System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "C:\\Users\\*\\Desktop\\pred\\dist\\predict.exe";
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = "\"C:\\Users\\*\\Desktop\\pred\\SavedModel\"" + " " + "\"" + item.Name + "\"";
            p.Start();*/


            /*System.Diagnostics.Process p = new System.Diagnostics.Process();
            
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "C:\\Users\\*\\Desktop\\heatmap\\dist\\heatmap.exe";
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = "\"C:\\Users\\*\\Desktop\\heatmap\\dist\\xceptionshoulder.h5\"" + " " + "\"" + item.Name + "\"";
            p.Start();*/



            /*await p.WaitForExitAsync();
            string output = p.StandardOutput.ReadToEnd();

            File.Delete(item.Name);

            if (output.Equals("1\r\n"))
                return true;
            else
                return false;*/
            return false;
        }

        public async Task<List<FileMaskModel>> LabelManageAsync(string Id)
        {
            
            var masks = identityContext.FileMasks.Where(i => i.FileItemId == Id).ToList();
            return masks;
        }
    }
}
