using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jogo.Model.Enum;

namespace Jogo.Views
{
    public interface IViewBase
    {
        Cenario Cenario { get; }
    }
}
