using KinectMath.Core.TermosEquacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectMath.Core
{
    public static class IntExtensions
    {
        public static TermoEquacao Unidades(this int valor)
        {
            return new Unidade(valor);
        }

        public static TermoEquacao X(this int valor)
        {
            return new Incognita(valor);
        }
    }
}
