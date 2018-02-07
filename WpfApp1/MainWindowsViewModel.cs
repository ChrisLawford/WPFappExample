using OxTS.NavLib.Common.Measurement;
using OxTS.NavLib.DataStoreManager.Manager;
using OxTS.NavLib.StreamItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class MainWindowsViewModel : ObservableObject
    {

        public MainWindowsViewModel()
        {
            m_random = new Random();
        }

        public string CrazyString
        {
            get
            {
                return m_crazy_string;
            }
            set
            {
                OnPropertyChanged("CrazyString");
                m_crazy_string = value;
            }
        }

        public string FileEntryString
        {
            get
            {
                return m_file_location;
            }
            set
            {
                OnPropertyChanged("FileEntryString");
                m_file_location = value;
            }
        }



        #region Members

        private string m_crazy_string;
        private Random m_random;
        private string m_file_location;

        #endregion




        public void UpdateString()
        {
            CrazyString = String.Format("well done sir!! heres a random no: " + m_random.Next());
        }

        public void OpenFile()
        {
            //create new realtime data store manager
            FileDataStoreManager file_ds_manager = new FileDataStoreManager();

            file_ds_manager.AddFile(FileEntryString);
           
            //increment of 100000000ns, gives us 1Hz
            file_ds_manager.DecodeData(file_ds_manager.GetStoreStartTime(), file_ds_manager.GetStoreEndTime(), 100000000);


            //Get all available streams in the data store
            List<IStreamItem> streams = file_ds_manager.GetAllStreamItems();

            //For each stream in datastore
            foreach (IStreamItem stream in streams)
            {
                //System.Console.Clear();
                //Output stream information

                    FileEntryString = String.Format("File name: " + stream.Address +"\n");
                
                    FileEntryString += String.Format("Resource ID: " + stream.ResourceId +"\n");
                    FileEntryString += String.Format("Stream ID: " + stream.StreamId + "\n");
                    FileEntryString += String.Format("Stream Name: " + stream.StreamName + "\n");
                    FileEntryString += String.Format("Transport Name: " + stream.TransportName + "\n");
                    FileEntryString += String.Format("Stream Codec: " + stream.CodecName + "\n");
                    FileEntryString += String.Format("User Tag: " + stream.UserTag + "\n");
                    FileEntryString += String.Format("Device Serial number: " + stream.DeviceSerialNumber + "\n");


                //FileEntryString = String.Format("");
                //FileEntryString = String.Format("End of Stream Data");
                //FileEntryString = String.Format("Press Enter key for next stream...");
                //FileEntryString = String.Format("");
            }

            //No need to configure measurements or decode when using direct but we still need the list of measurements with stream id
            List<OxTS.NavLib.Common.Measurement.MeasurementItem> measurements = new List<OxTS.NavLib.Common.Measurement.MeasurementItem>();
            measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Nano"));
            measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Ax"));
            measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Ay"));
            measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Az"));


            Int64 current_time = file_ds_manager.GetStoreStartTime();
            Int64 end_time = file_ds_manager.GetStoreEndTime();
            Int64 One_Second = 1000000000;

            //Tell the data store to seek to the correct position
            file_ds_manager.BeginDirectDecode(current_time);

            //For each stream in datastore
            while (current_time < end_time)
            {
                List<MeasurementValue> meas_data = file_ds_manager.GetDirectStreamDataForInstant(current_time, streams[0].StreamId, measurements);

                foreach (MeasurementValue data in meas_data)
                {
                    FileEntryString += String.Format( data.MeasurementName + " : " + data.ValueString + "\n");
                }

                current_time += One_Second;
            }

            //close handles to stream data
            file_ds_manager.EndDirectDecode();
        }
        //FileEntryString = "Well Done you did something";

    }

    //    //create new realtime data store manager
    //    FileDataStoreManager file_ds_manager = new FileDataStoreManager();

    //    file_ds_manager.AddFile(@"..\..\..\..\Example_Files\example1.ncom");
    //        file_ds_manager.AddFile(@"..\..\..\..\Example_Files\example2.ncom");

    //        //increment of 100000000ns, gives us 1Hz
    //        file_ds_manager.DecodeData(file_ds_manager.GetStoreStartTime(), file_ds_manager.GetStoreEndTime(), 100000000);


    //        //Get all available streams in the data store
    //        List<IStreamItem> streams = file_ds_manager.GetAllStreamItems();

    //        //For each stream in datastore
    //        foreach (IStreamItem stream in streams)
    //        {
    //            System.Console.Clear();
    //            //Output stream information
    //            System.Console.WriteLine("File name: " + stream.Address);
    //            System.Console.WriteLine("Resource ID: " + stream.ResourceId);
    //            System.Console.WriteLine("Stream ID: " + stream.StreamId);
    //            System.Console.WriteLine("Stream Name: " + stream.StreamName);
    //            System.Console.WriteLine("Transport Name: " + stream.TransportName);
    //            System.Console.WriteLine("Stream Codec: " + stream.CodecName);
    //            System.Console.WriteLine("User Tag: " + stream.UserTag);
    //            System.Console.WriteLine("Device Serial number: " + stream.DeviceSerialNumber);

    //            System.Console.WriteLine();
    //            System.Console.WriteLine("End of Stream Data");
    //            System.Console.WriteLine("Press Enter key for next stream...");
    //            System.Console.ReadLine();
    //        }

    //System.Console.Clear();
    //        System.Console.WriteLine("End of Streams");
    //        System.Console.WriteLine("Press Enter to start again...");
    //        System.Console.ReadLine();


//    //Get all available streams in the data store
//    List<IStreamItem> streams = file_ds_manager.GetAllStreamItems();

//    //No need to configure measurements or decode when using direct but we still need the list of measurements with stream id
//    List<OxTS.NavLib.Common.Measurement.MeasurementItem> measurements = new List<OxTS.NavLib.Common.Measurement.MeasurementItem>();
//    measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Nano"));
//            measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Ax"));
//            measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Ay"));
//            measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Az"));


//            Int64 current_time = file_ds_manager.GetStoreStartTime();
//    Int64 end_time = file_ds_manager.GetStoreEndTime();
//    Int64 One_Second = 1000000000;

//    //Tell the data store to seek to the correct position
//    file_ds_manager.BeginDirectDecode(current_time);

//            //For each stream in datastore
//            while (current_time<end_time)
//            {
//                List<MeasurementValue> meas_data = file_ds_manager.GetDirectStreamDataForInstant(current_time, streams[0].StreamId, measurements);

//                foreach (MeasurementValue data in meas_data)
//                {
//                    System.Console.WriteLine(data.MeasurementName + " : " + data.ValueString);
//                }

//System.Console.WriteLine();

//                current_time += One_Second;
//            }

//            //close handles to stream data
//            file_ds_manager.EndDirectDecode();
        
    
}
