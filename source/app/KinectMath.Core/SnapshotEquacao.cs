namespace KinectMath.Core
{
    public class SnapshotEquacao
    {
        private readonly int unidadesEsquerda;
        private readonly int unidadesDireita;
        private readonly int incognitasDireita;
        private readonly int incognitasEsquerda;

        public SnapshotEquacao(int unidadesEsquerda, int incognitasEsquerda, int unidadesDireita, int incognitasDireita)
        {
            this.unidadesEsquerda = unidadesEsquerda;
            this.unidadesDireita = unidadesDireita;
            this.incognitasDireita = incognitasDireita;
            this.incognitasEsquerda = incognitasEsquerda;
        }

        public int UnidadesEsquerda
        {
            get { return unidadesEsquerda; }
        }

        public int IncognitasEsquerda
        {
            get { return incognitasEsquerda; }
        }

        public int UnidadesDireita
        {
            get { return unidadesDireita; }
        }

        public int IncognitasDireita
        {
            get { return incognitasDireita; }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            var p = obj as SnapshotEquacao;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (unidadesEsquerda == p.unidadesEsquerda)
                && (unidadesDireita == p.unidadesDireita)
                && (incognitasEsquerda == p.incognitasEsquerda)
                && (incognitasDireita == p.incognitasDireita);
        }

        public bool Equals(SnapshotEquacao p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (unidadesEsquerda == p.unidadesEsquerda)
                && (unidadesDireita == p.unidadesDireita)
                && (incognitasEsquerda == p.incognitasEsquerda)
                && (incognitasDireita == p.incognitasDireita);
        }

        public override int GetHashCode()
        {
            return unidadesEsquerda ^ unidadesEsquerda ^ IncognitasEsquerda ^ IncognitasDireita;
        }
    }
}