using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraficadorSenales
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        double amplitudMaxima = 1;

        Senal senal;
        Senal senalResultado;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGraficar_Click(object sender, RoutedEventArgs e)
        {
            double tiempoInicial = double.Parse(txtTiempoInicial.Text);
            double tiempoFinal = double.Parse(txtTiempoFinal.Text);
            double frecMuestreo = double.Parse(txtFrecMuestreo.Text);

            //Primer señal.
            switch (cbTipoSenal.SelectedIndex)
            {
                case 0: //Senoidal.
                    double amplitud = double.Parse(((ConfiguracionSenoidal)(panelConfiguracion.Children[0])).txtAmplitud.Text);
                    double fase = double.Parse(((ConfiguracionSenoidal)(panelConfiguracion.Children[0])).txtFase.Text);
                    double frecuencia = double.Parse(((ConfiguracionSenoidal)(panelConfiguracion.Children[0])).txtFrecuencia.Text);

                    senal = new SenalSenoidal(amplitud, fase, frecuencia);
                    break;

                case 1: //Rampa.
                    senal = new SenalRampa();
                    break;

                case 2: //Exponencial.
                    double alfa = double.Parse(((ConfiguracionExponencial)(panelConfiguracion.Children[0])).txtAlfa.Text);

                    senal = new SenalExponencial(alfa);
                    break;

                case 3: //Rectangular
                    senal = new SenalRectangular();
                    break;

                default: senal = null;
                    break;
            }

            senal.TiempoInicial = tiempoInicial;
            senal.TiempoFinal = tiempoFinal;
            senal.FrecMuestreo = frecMuestreo;

            //Construye señal
            senal.construirSenalDigital();

            //Escalar PRIMERA SEÑAL.
            double factorEscala = double.Parse(txtFactorEscalaAmplitud.Text);
            senal.escalar(factorEscala);
            //Desplazar PRIMERA SEÑAL.
            double factorDesplazar = double.Parse(txtFactorDesplazamiento.Text);
            senal.desplazar(factorDesplazar);
            senal.actualizarAmplitudMaxima();
            //Truncar PRIMERA SEÑAL.
            /* double factorTruncar = double.Parse(txtUmbral.Text);
            senal.truncar(factorTruncar); */

            //Establecer amplitud máxima.
            senal.actualizarAmplitudMaxima();
            amplitudMaxima = senal.amplitudMaxima;

            //Limpia las gráficas.
            plnGrafica.Points.Clear();

            lblAmplitudMaximaY.Text = amplitudMaxima.ToString("F");
            lblAmplitudMaximaY_Negativa.Text = "-" + amplitudMaxima.ToString("F");

            //Graficando PRIMERA señal.
            if (senal != null)
            {
                //Recorrer una colección o arreglo.
                foreach (Muestra muestra in senal.Muestras)
                {
                    plnGrafica.Points.Add(new Point((muestra.x - tiempoInicial) * scrContenedor.Width,
                        (muestra.y / amplitudMaxima) * ((scrContenedor.Height / 2.0) - 30) * -1 + (scrContenedor.Height / 2)));
                }
            }
       
            //Graficando el eje de X
            plnEjeX.Points.Clear();
            //Punto de inicio.
            plnEjeX.Points.Add(new Point(0, (scrContenedor.Height / 2)));
            //Punto de fin.
            plnEjeX.Points.Add(new Point((tiempoFinal - tiempoInicial) * scrContenedor.Width, (scrContenedor.Height / 2)));

            //Graficando el eje de Y
            plnEjeY.Points.Clear();
            //Punto de inicio.
            plnEjeY.Points.Add(new Point(0 - tiempoInicial * scrContenedor.Width, scrContenedor.Height));
            //Punto de fin.
            plnEjeY.Points.Add(new Point(0 - tiempoInicial * scrContenedor.Width, scrContenedor.Height * -1));
        }

        private void cbTipoSenal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (panelConfiguracion != null)
            {
                panelConfiguracion.Children.Clear();

                switch (cbTipoSenal.SelectedIndex)
                {
                    case 0: //Senoidal
                        panelConfiguracion.Children.Add(new ConfiguracionSenoidal());
                        break;

                    case 1: //Rampa
                        break;

                    case 2: //Exponencial
                        panelConfiguracion.Children.Add(new ConfiguracionExponencial());
                        break;

                    case 3: //Rectangular
                        break;

                    default:
                        break;
                }
            }
        }

        //Primera señal
        private void cbAmplitud_Checked(object sender, RoutedEventArgs e)
        {
            txtFactorEscalaAmplitud.IsEnabled = true;

        }
        private void cbAmplitud_UnChecked(object sender, RoutedEventArgs e)
        {
            txtFactorEscalaAmplitud.IsEnabled = false;
            txtFactorEscalaAmplitud.Text = "1";
        }

        private void cbDesplazamiento_Checked(object sender, RoutedEventArgs e)
        {
            txtFactorDesplazamiento.IsEnabled = true;
        }
        private void cbDesplazamiento_UnChecked(object sender, RoutedEventArgs e)
        {
            txtFactorDesplazamiento.IsEnabled = false;
            txtFactorDesplazamiento.Text = "0";
        }

        private void cbUmbral_Checked(object sender, RoutedEventArgs e)
        {
            txtUmbral.IsEnabled = true;
        }
        private void cbUmbral_UnChecked(object sender, RoutedEventArgs e)
        {
            txtUmbral.IsEnabled = false;
            txtUmbral.Text = "1";
        }

        private void btnFourier_Click(object sender, RoutedEventArgs e)
        {
            Senal transformada =
                Senal.transformar(senal);

            //Limpia las gráficas.
            plnGraficaResultado.Points.Clear();

            lblAmplitudMaximaY_Copy.Text = transformada.amplitudMaxima.ToString("F");
            lblAmplitudMaximaY_Negativa_Copy.Text = "-" + transformada.amplitudMaxima.ToString("F");

            //Graficando PRIMERA señal.
            if (transformada != null)
            {
                //Recorrer una colección o arreglo.
                foreach (Muestra muestra in transformada.Muestras)
                {
                    plnGraficaResultado.Points.Add(new Point((muestra.x - transformada.TiempoInicial) * scrResultadoOperacion.Width,
                        (muestra.y / transformada.amplitudMaxima) * ((scrResultadoOperacion.Height / 2.0) - 30) * -1 + (scrResultadoOperacion.Height / 2)));
                }
            }

            //Graficando el eje de X
            plnEjeXResultado.Points.Clear();
            //Punto de inicio.
            plnEjeXResultado.Points.Add(new Point(0, (scrResultadoOperacion.Height / 2)));
            //Punto de fin.
            plnEjeXResultado.Points.Add(new Point((transformada.TiempoFinal - transformada.TiempoInicial) * scrResultadoOperacion.Width, (scrResultadoOperacion.Height / 2)));

            //Graficando el eje de Y
            plnEjeYResultado.Points.Clear();
            //Punto de inicio.
            plnEjeYResultado.Points.Add(new Point(0 - transformada.TiempoInicial * scrResultadoOperacion.Width, scrResultadoOperacion.Height));
            //Punto de fin.
            plnEjeYResultado.Points.Add(new Point(0 - transformada.TiempoInicial * scrResultadoOperacion.Width, scrResultadoOperacion.Height * -1));
        }
    }
}
