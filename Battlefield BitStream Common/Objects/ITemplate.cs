using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Objects
{
    public interface ITemplate
    {
        int TemplateId { get; set; }
        string TemplateName { get; set; }
    }
}