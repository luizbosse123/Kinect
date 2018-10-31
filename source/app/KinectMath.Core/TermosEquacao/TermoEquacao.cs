using System;

namespace KinectMath.Core.TermosEquacao
{
    public abstract class TermoEquacao : IEquatable<TermoEquacao>
    {
        public TipoTermo Tipo { get; internal set; }
        public int Valor { get; internal set; }
        public int Potencia { get; set; }

        public TermoEquacao(TipoTermo tipo, int valor)
        {
            Tipo = tipo;
            Valor = valor;
            Potencia = 1;
        }

        public static bool operator ==(TermoEquacao obj1, TermoEquacao obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.Tipo == obj2.Tipo && obj1.Valor == obj2.Valor && obj1.Potencia == obj2.Potencia;
        }

        // this is second one '!='
        public static bool operator !=(TermoEquacao obj1, TermoEquacao obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(TermoEquacao other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Tipo.Equals(other.Tipo) && Valor.Equals(other.Valor) && Potencia.Equals(other.Potencia);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((TermoEquacao)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Valor.GetHashCode();
                hashCode = (hashCode * 397) ^ Tipo.GetHashCode();
                hashCode = (hashCode * 397) ^ Potencia.GetHashCode();
                return hashCode;
            }
        }
    }
}
