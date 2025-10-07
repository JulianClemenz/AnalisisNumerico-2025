using System;

public class DatoEntradaRPDto
{
	public List<double[]> puntosCargados { get; set; }//puntos de entrada
    public double Tolerancia { get; set; }//0.8
    public bool metodo { get; set; }
    public int grado { get; set; }//define el grado del polinomio
}
