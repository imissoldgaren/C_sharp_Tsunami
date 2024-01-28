using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HDF.PInvoke;

using System.Globalization;

namespace myLibrary
{
    public class HDF5
    {
        public static void CreateHdf5File(string filename, ulong nx, ulong ny)
    {
        try
        {
            // Number of time steps and other parameters
            int numTimeSteps = 50;

            // Create HDF5 file
            long fileId = H5F.create(filename, H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT);

            // Create dataspace for dimensions x and y
            ulong[] dimsX = { nx };
            ulong[] dimsY = { ny };
            ulong[] dimsTime = { (ulong)numTimeSteps };

            long dimXSpaceId = H5S.create_simple(1, dimsX, null);
            long dimYSpaceId = H5S.create_simple(1, dimsY, null);
            long dimTimeSpaceId = H5S.create_simple(1, dimsTime, null);

            // Create dataspace for the 3D dataset
            ulong[] dims3D = { (ulong)numTimeSteps, nx, ny };
            long dim3DSpaceId = H5S.create_simple(3, dims3D, null);

            // Create datasets for x, y, and time dimensions
            long varIdX = H5D.create(fileId, "x", H5T.IEEE_F32LE, dimXSpaceId, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);
            long varIdY = H5D.create(fileId, "y", H5T.IEEE_F32LE, dimYSpaceId, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);
            long varIdTime = H5D.create(fileId, "time", H5T.IEEE_F32LE, dimTimeSpaceId, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);

            // Create 3D dataset for height
            long varIdHeight = H5D.create(fileId, "height", H5T.IEEE_F32LE, dim3DSpaceId, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);

            // Create 3D dataset for momentum in x direction (hu)
            long varIdImpulseX = H5D.create(fileId, "hu", H5T.IEEE_F32LE, dim3DSpaceId, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);

            // Create 3D dataset for momentum in y direction (hv)
            long varIdImpulseY = H5D.create(fileId, "hv", H5T.IEEE_F32LE, dim3DSpaceId, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);

            long varIdBathymetry = H5D.create(fileId, "hv", H5T.IEEE_F32LE, dim3DSpaceId, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);


            // Close resources
            H5D.close(varIdX);
            H5D.close(varIdY);
            H5D.close(varIdTime);
            H5D.close(varIdHeight);
            H5D.close(varIdImpulseX);
            H5D.close(varIdImpulseY);
            H5S.close(dimXSpaceId);
            H5S.close(dimYSpaceId);
            H5S.close(dimTimeSpaceId);
            H5S.close(dim3DSpaceId);
            H5F.close(fileId);

            Console.WriteLine("HDF5 file with dimensions x, y, time, height, hu, and hv created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
    


        private const string VarIdX = "x";
        private const string VarIdY = "y";
        private const string VarIdBathymetry = "bathymetry";

        public static void FillConstants(int nx, int ny, int stride, double dxy, double domainStartX, double domainStartY, int[] b, string filename)
        {
            long ncId, err;

            // Create a new HDF5 file
            ncId = H5F.create(filename, H5F.ACC_TRUNC);
            if (ncId < 0)
            {
                Console.WriteLine("Error creating file: " + filename);
                return;
            }

            // Create x and y coordinates datasets
            CreateCoordinateDataset(ncId, VarIdX, nx, dxy, domainStartX);
            CreateCoordinateDataset(ncId, VarIdY, ny, dxy, domainStartY);

            // Create and write bathymetry dataset
            CreateBathymetryDataset(ncId, VarIdBathymetry, b, nx, ny, stride);

            // Close the file
            err = H5F.close(ncId);
            if (err < 0)
            {
                Console.WriteLine("Error closing file: " + filename);
            }
        }

       private static void CreateCoordinateDataset(long ncId, string varId, int size, double dxy, double domainStart)
{
    int[] coordinateData = new int[size ];

    for (int i = 0; i < size ; i++)
    {
        coordinateData[i] = (int)(((i + 0.5) * dxy ) + domainStart);
    }

    // Create the dataset
    var dataspaceId = H5S.create_simple(1, new ulong[] { (ulong)coordinateData.Length }, null);
    var datasetId = H5D.create(ncId, varId, H5T.NATIVE_INT, dataspaceId);

    // Write data to the dataset
    H5D.write(datasetId, H5T.NATIVE_INT, dataspaceId, dataspaceId, H5P.DEFAULT, coordinateData[0]);

    // Close resources
    H5D.close(datasetId);
    H5S.close(dataspaceId);
}

        private static void CreateBathymetryDataset(long ncId, string varId, int[] b, int nx, int ny, int stride)
        {
            // Create the dataset
            var dataspaceId = H5S.create_simple(2, new ulong[] { (ulong)(nx ), (ulong)(ny ) }, null);
            var datasetId = H5D.create(ncId, varId, H5T.NATIVE_INT, dataspaceId);

            // Write data to the dataset
            H5D.write(datasetId, H5T.NATIVE_INT, dataspaceId, dataspaceId, H5P.DEFAULT, b[0]);

            // Close resources
            H5D.close(datasetId);
            H5S.close(dataspaceId);
        }
    }
}
