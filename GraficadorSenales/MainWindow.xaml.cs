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
        Senal senal2;
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

            //Segunda señal.
            switch (cbTipoSenal_2.SelectedIndex)
            {
                case 0: //Senoidal.
                    double amplitud = double.Parse(((ConfiguracionSenoidal)(panelConfiguracion_2.Children[0])).txtAmplitud.Text);
                    double fase = double.Parse(((ConfiguracionSenoidal)(panelConfiguracion_2.Children[0])).txtFase.Text);
                    double frecuencia = double.Parse(((ConfiguracionSenoidal)(panelConfiguracion_2.Children[0])).txtFrecuencia.Text);

                    senal2 = new SenalSenoidal(amplitud, fase, frecuencia);
                    break;

                case 1: //Rampa.
                    senal2 = new SenalRampa();
                    break;

                case 2: //Exponencial.
                    double alfa = double.Parse(((ConfiguracionExponencial)(panelConfiguracion_2.Children[0])).txtAlfa.Text);

                    senal2 = new SenalExponencial(alfa);
                    break;

                case 3: //Rectangular
                    senal2 = new SenalRectangular();
                    break;

                default:
                    senal2 = null;
                    break;
            }

            senal2.TiempoInicial = tiempoInicial;
            senal2.TiempoFinal = tiempoFinal;
            senal2.FrecMuestreo = frecMuestreo;

            //Construye señal
            senal.construirSenalDigital();
            senal2.construirSenalDigital();

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

            //Escalar SEGUNDA SEÑAL.
            double factorEscala2 = double.Parse(txtFactorEscalaAmplitud_2.Text);
            senal2.escalar(factorEscala2);
            //Desplazar SEGUNDA SEÑAL.
            double factorDesplazar2 = double.Parse(txtFactorDesplazamiento_2.Text);
            senal2.desplazar(factorDesplazar2);
            senal2.actualizarAmplitudMaxima();
            //Truncar SEGUNDA SEÑAL.
            /* double factorTruncar2 = double.Parse(txtUmbral_2.Text);
            senal2.truncar(factorTruncar2); */

            //Establecer amplitud máxima comparando con AMBAS señales.
            senal.actualizarAmplitudMaxima();
            senal2.actualizarAmplitudMaxima();

            //Definimos la amplitud máxima con la primer señal por default y comparamos con la segunda en caso de ser mayor.
            senal.actualizarAmplitudMaxima();
            senal2.actualizarAmplitudMaxima();
            amplitudMaxima = senal.amplitudMaxima;

            if (senal2.amplitudMaxima > amplitudMaxima) amplitudMaxima = senal2.amplitudMaxima;

            //Limpia las gráficas.
            plnGrafica.Points.Clear();
            plnGrafica_2.Points.Clear();

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


            //Graficando SEGUNDA señal.
            if (senal2 != null)
            {
                //Recorrer una colección o arreglo.
                foreach (Muestra muestra in senal2.Muestras)
                {
                    plnGrafica_2.Points.Add(new Point((muestra.x - tiempoInicial) * scrContenedor.Width,
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

        private void cbTipoSenal_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (panelConfiguracion_2 != null)
            {
                panelConfiguracion_2.Children.Clear();

                switch (cbTipoSenal_2.SelectedIndex)
                {
                    case 0: //Senoidal
                        panelConfiguracion_2.Children.Add(new ConfiguracionSenoidal());
                        break;

                    case 1: //Rampa
                        break;

                    case 2: //Exponencial
                        panelConfiguracion_2.Children.Add(new ConfiguracionExponencial());
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

        //Segunda señal
        private void cbAmplitud_2_Checked(object sender, RoutedEventArgs e)
        {
            txtFactorEscalaAmplitud_2.IsEnabled = true;
        }
        private void cbAmplitud_2_UnChecked(object sender, RoutedEventArgs e)
        {
            txtFactorEscalaAmplitud_2.IsEnabled = false;
            txtFactorEscalaAmplitud_2.Text = "1";
        }

        private void cbDesplazamiento_2_Checked(object sender, RoutedEventArgs e)
        {
            txtFactorDesplazamiento_2.IsEnabled = true;
        }
        private void cbDesplazamiento_2_UnChecked(object sender, RoutedEventArgs e)
        {
            txtFactorDesplazamiento_2.IsEnabled = false;
            txtFactorDesplazamiento_2.Text = "0";
        }

        private void cbUmbral_2_Checked(object sender, RoutedEventArgs e)
        {
            txtUmbral_2.IsEnabled = true;
        }
        private void cbUmbral_2_UnChecked(object sender, RoutedEventArgs e)
        {
            txtUmbral_2.IsEnabled = false;
            txtUmbral_2.Text = "1";
        }

        private void btnRealizarOperacion_Click(object sender, RoutedEventArgs e)
        {
            senalResultado = null;
            switch (cbTipoOperacion.SelectedIndex)
            {
                case 0: //Suma
                    //los metodos estaticos no necesitan una instancia
                    senalResultado = Senal.sumar(senal, senal2);
                    break;
                case 1: //Multiplicación
                    senalResultado = Senal.multiplicar(senal, senal2);
                    break;
                case 2: //Convolución
                    senalResultado = Senal.convolucionar(senal, senal2);
                    break;
                default:
                    break;
            }
            //se actualiza la amplitud maxima del resultado 
            senalResultado.actualizarAmplitudMaxima();
            plnGraficaResultado.Points.Clear();
            //cambia los valores de la etiqueta
            //La F es que da el formato para redondear a 2 decimales, la funcion ToString puede recibir un parametro que es el que va a decidir en que formato va a estar,existen varios parametros
            lblAmplitudMaximaY_Copy.Text = senalResultado.amplitudMaxima.ToString("F");
            lblAmplitudMaximaY_Negativa_Copy.Text = "-" + senalResultado.amplitudMaxima.ToString("F");
            //hacerlo si la señal no es nula
            if (senalResultado != null)
            {
                //recorrer una coleccion o arreglo
                //muestra toma el valor de señal.muestra en cada recorrido del ciclo
                foreach (Muestra muestra in senalResultado.Muestras)
                {
                    //se evalua la señal, luego se ajusta y de ahi se agrega el punto
                    plnGraficaResultado.Points.Add(new Point((muestra.x - senalResultado.TiempoInicial) * scrResultadoOperacion.Width, (muestra.y / senalResultado.amplitudMaxima * ((scrResultadoOperacion.Height / 2) - 30) * -1) + (scrResultadoOperacion.Height / 2)));
                }
            }

            //Graficando el eje de X
            plnEjeXResultado.Points.Clear();
            //Punto de inicio.
            plnEjeXResultado.Points.Add(new Point(0, (scrResultadoOperacion.Height / 2)));
            //Punto de fin.
            plnEjeXResultado.Points.Add(new Point((senalResultado.TiempoFinal - senalResultado.TiempoInicial) * scrResultadoOperacion.Width, (scrResultadoOperacion.Height / 2)));
            //Graficando el eje de Y
            plnEjeYResultado.Points.Clear();
            //Punto de inicio.
            plnEjeYResultado.Points.Add(new Point(0 - senalResultado.TiempoInicial * scrResultadoOperacion.Width, scrResultadoOperacion.Height));
            //Punto de fin.
            plnEjeYResultado.Points.Add(new Point(0 - senalResultado.TiempoInicial * scrResultadoOperacion.Width, scrResultadoOperacion.Height * -1));
        }
    }
}
