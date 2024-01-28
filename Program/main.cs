

using System.Globalization;
using myLibrary;
using System;
using HDF;
using HDF.PInvoke;
using System.Text;




class Program
{
    

     

    public static void Main(string[] args) {

        float l_hl = 10;
        float l_hr = 5;
        float location = 5;
        uint nx = 100;
        uint ny = 1;
        uint l_dimensionx = 440500 ;
        //int l_dimensiony = 100;
        int domainstartx = 0;
        int domainstarty = 0;
        float d_xy = l_dimensionx / nx;
        float l_hMax = float.MinValue;
        float l_simTime = 0;
        float endTime = 500;
        float l_timeStep = 0;
        float l_time_step_index = 0;
      


        

        WavePropagation1d wave = new WavePropagation1d(nx, false);
        //ISetup setup = new DamBreak1d(l_hl, l_hr, location);

        //WavePropagation2d wave = new WavePropagation2d(nx,ny,false);
        ISetup setup = new TsunamiEvent1d();
        
        

    
        string columnNames = "x,y,h,hu,b";


        for (uint i = 0; i < ny; i++)
        {
            for (uint j = 0; j < nx; j++)
            {
                float l_y = i * d_xy + domainstartx;
                float l_x = j * d_xy + domainstarty;


                float l_h = setup.GetHeight(l_x, l_y);
                float l_hu = setup.GetMomentumX(l_x, l_y);
                float l_bv = setup.GetBathymetry(l_x, l_y);

                l_hMax = Math.Max(l_h, l_hMax);

                wave.setHeight(j, i, l_h);
                wave.setMomentumX(j, i, l_hu);
               //wave.setMomentumY(j, i, l_hv);
                wave.setBathymetry(j, i, l_bv);
            }
        }

       
        float l_speedMax = MathF.Sqrt(9.81f * l_hMax);
        float l_dt = 0.45f * d_xy / l_speedMax;
        float l_scaling = l_dt / d_xy;

        
        List<float> list = new List<float>();
        list= IO.Read("C:\\Users\\khale\\OneDrive\\Desktop\\project\\myLibrary\\data_end.csv",3);

        list.ToList().ForEach(element => Console.Write($" ==> {element}"));

        
        while (l_simTime < endTime){
            if(l_timeStep % 20 == 0){
                string l_path = $"../outputs/solution_{l_time_step_index}.csv";
                using (StreamWriter l_file = new StreamWriter(l_path)){
                        CultureInfo culture = CultureInfo.InvariantCulture;
                        IO.Write1d(d_xy, nx, ny, wave.GetStride(), domainstartx, domainstarty, wave.getHeightValues(), wave.getMomentumXValues().ToArray(), wave.getBathymetryValues(), l_file);
                       // IO.Write2d(d_xy, nx, ny, wave.GetStride(), domainstartx, domainstarty, wave.getHeightValues(), wave.getMomentumXValues().ToArray(),wave.getMomentumYValues().ToArray(), wave.getBathymetryValues(), l_file);

                }
                        

                            l_time_step_index++;
                }
                
                wave.timeStep(l_scaling);
                l_timeStep++;
                l_simTime += l_dt;
            }
        }

}

