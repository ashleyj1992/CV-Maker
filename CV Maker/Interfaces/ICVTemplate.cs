using CV_Maker.Models;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Maker.Interfaces
{
    public interface ICVTemplate : IDocument
    {
        CV CV { get; set; }
    }
}
