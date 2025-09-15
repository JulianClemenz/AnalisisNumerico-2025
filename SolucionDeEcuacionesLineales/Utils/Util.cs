namespace Utils
{
    public class Util
    {
        public static (bool esDD, List<int> filasNoDD) EsDiagonalmenteDominante(double[][] m)
        {
            var malas = new List<int>();
            for (int i = 0; i < m.Length; i++)
            {
                double diag = Math.Abs(m[i][i]);
                double suma = 0;
                for (int j = 0; j < m[i].Length - 1; j++) // solo coeficientes, sin TI
                    if (j != i) suma += Math.Abs(m[i][j]);

                if (diag <= suma) malas.Add(i); // no cumple DD estricta
            }
            return (malas.Count == 0, malas);
        }
    }
}
