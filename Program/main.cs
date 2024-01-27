using System.Globalization;

using myLibrary;

class Program
{

        public static  void Write2d(float i_dxy,
                                 uint i_nx,
                                 uint i_ny,
                                 uint i_stride,
                                 int i_domainstart_x,
                                 int i_domainstart_y,
                                 float[] i_h,
                                 float[] i_hu,
                                 float[] i_hv,
                                 float[] i_b,
                                 StreamWriter io_stream)


    {
        uint l_id = 0;
        CultureInfo culture = CultureInfo.InvariantCulture;
        // Write the CSV header
        io_stream.WriteLine("x,y" +
                         (i_h != null ? ",height" : "") +
                         (i_hu != null ? ",momentum_x" : "") +
                         (i_hv != null ? ",momentum_y" : "") +
                         (i_b != null ? ",bathymetry" : ""));//+
                           // (l_id != null ? ",l_id" : ""));


        for (uint l_iy = 1; l_iy < i_ny + 1; l_iy++)
        {
            for (uint l_ix = 1; l_ix < i_nx + 1; l_ix++)
            {
                // Derive coordinates
                float l_posX = ((l_ix - 1 + 0.5f) * i_dxy) + i_domainstart_x;
                float l_posY = ((l_iy - 1 + 0.5f) * i_dxy) + i_domainstart_y;

           

                 l_id = (l_iy * i_stride) + l_ix;

                 if(l_posX == 45.5f){
                    //Console.WriteLine("l_ix: " + l_ix + " " +  "l_iy: " + l_iy + " " +  "l_id: " + l_id);
                 }

                io_stream.WriteLine($"{l_posX.ToString(culture)},{l_posY.ToString(culture)}" +
                                              (i_h != null ? $",{i_h[l_id].ToString(culture)}" : "") +
                                              (i_hu != null ? $",{i_hu[l_id].ToString(culture)}" : "") +
                                              (i_hv != null ? $",{i_hv[l_id].ToString(culture)}" : "") +
                                              (i_b != null ? $",{i_b[l_id].ToString(culture)}" : "")); //+
                                              //(l_id != null ? $",{l_id.ToString(culture)}" : ""));
            }
        }

        io_stream.Flush();


    }

     public static  void Write1d(float i_dxy,
                                 uint i_nx,
                                 uint i_ny,
                                 uint i_stride,
                                 int i_domainstart_x,
                                 int i_domainstart_y,
                                 float[] i_h,
                                 float[] i_hu,
                                 float[] i_b,
                                 StreamWriter io_stream)


    {
        uint l_id = 0;
        CultureInfo culture = CultureInfo.InvariantCulture;
        // Write the CSV header
        io_stream.WriteLine("x,y" +
                         (i_h != null ? ",height" : "") +
                         (i_hu != null ? ",momentum_x" : "") +
                         (i_b != null ? ",bathymetry" : ""));//+
                           // (l_id != null ? ",l_id" : ""));


        for (uint l_iy = 0; l_iy < i_ny ; l_iy++)
        {
            for (uint l_ix = 0; l_ix < i_nx ; l_ix++)
            {
                // Derive coordinates
                float l_posX = ((l_ix  + 0.5f) * i_dxy) + i_domainstart_x;
                float l_posY = ((l_iy  + 0.5f) * i_dxy) + i_domainstart_y;

           

                 l_id = (l_iy * i_stride) + l_ix;

                 if(l_posX == 45.5f){
                    //Console.WriteLine("l_ix: " + l_ix + " " +  "l_iy: " + l_iy + " " +  "l_id: " + l_id);
                 }

                io_stream.WriteLine($"{l_posX.ToString(culture)},{l_posY.ToString(culture)}" +
                                              (i_h != null ? $",{i_h[l_id].ToString(culture)}" : "") +
                                              (i_hu != null ? $",{i_hu[l_id].ToString(culture)}" : "") +
                                              (i_b != null ? $",{i_b[l_id].ToString(culture)}" : "")); //+
                                              //(l_id != null ? $",{l_id.ToString(culture)}" : ""));
            }
        }

        io_stream.Flush();


    }


    public static void Main(string[] args) {

        float l_hl = 10;
        float l_hr = 5;
        float location = 5;
        uint nx = 100;
        uint ny = 1;
        uint l_dimensionx = 100;
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
        ISetup setup = new DamBreak1d(l_hl,l_hr,location);
        
        

    
        string columnNames = "x,y,h,hu,b";


        for (uint i = 0; i < ny; i++)
        {
            for (uint j = 0; j < nx; j++)
            {
                float l_y = i * d_xy + domainstartx;
                float l_x = j * d_xy + domainstarty;


                float l_h = setup.GetHeight(l_x, l_y);
                float l_hu = setup.GetMomentumX(l_x, l_y);
                //float l_hv = setup.GetMomentumY(l_x, l_y);
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

    
        
        while (l_simTime < endTime){
            if(l_timeStep % 20 == 0){
                string l_path = $"../outputs/solution_{l_time_step_index}.csv";
                using (StreamWriter l_file = new StreamWriter(l_path)){
                        CultureInfo culture = CultureInfo.InvariantCulture;
                        Write1d(d_xy, nx, ny, wave.GetStride(), domainstartx, domainstarty, wave.getHeightValues(), wave.getMomentumXValues().ToArray(), wave.getBathymetryValues(), l_file);
                       // Write2d(d_xy, nx, ny, wave.GetStride(), domainstartx, domainstarty, wave.getHeightValues(), wave.getMomentumXValues().ToArray(),wave.getMomentumYValues().ToArray(), wave.getBathymetryValues(), l_file);

                }
                        

                            l_time_step_index++;
                }
                
                wave.timeStep(l_scaling);
                l_timeStep++;
                l_simTime += l_dt;
            }
        }

}

