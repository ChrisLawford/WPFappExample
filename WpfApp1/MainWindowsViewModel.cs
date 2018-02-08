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
        }

        #region Properties

        public string FileEntryString  {get{    return m_file_location;}        set{    OnPropertyChanged("FileEntryString");   m_file_location         = value;}}
        public string OutputNanoString {get{    return m_outputstring_location;}set{    OnPropertyChanged("OutputNanoString");  m_outputstring_location = value;}}
        public string OutputAxString   {get{    return m_output_ax_string;}     set{    OnPropertyChanged("OutputAxString");    m_output_ax_string      = value;}}
        public string OutputAyString   {get{    return m_output_ay_string;}     set{    OnPropertyChanged("OutputAyString");    m_output_ay_string      = value;}}
        public string OutputAzString   {get{    return m_output_az_string;}     set{    OnPropertyChanged("OutputAzString");    m_output_az_string      = value;}}

        #endregion

        #region Members

        private string m_file_location;
        private string m_output_ax_string;
        private string m_output_ay_string;
        private string m_output_az_string;
        private string m_outputstring_location;
        private Int64 i = 0;
        private Int64 current_time;
        private Int64 end_time;
        private Boolean Setup = false;


        #endregion

        //create new realtime data store manager
        FileDataStoreManager file_ds_manager = new FileDataStoreManager();
        //Create measurements list
        List<OxTS.NavLib.Common.Measurement.MeasurementItem> measurements = new List<OxTS.NavLib.Common.Measurement.MeasurementItem>();
        List<IStreamItem> streams;

        public void OpenFilePlusSetup()
        {
            if (Setup == false)
            {
                //Add the file to the datastoremanager
                file_ds_manager.AddFile(FileEntryString);

                //increment of 100000000ns, gives us 1Hz
                file_ds_manager.DecodeData(file_ds_manager.GetStoreStartTime(), file_ds_manager.GetStoreEndTime(), 100000000);


                //Get all available streams in the data store
                streams = file_ds_manager.GetAllStreamItems();

                //For each stream in datastore
                foreach (IStreamItem stream in streams)
                {
                    //Output stream information

                    FileEntryString = String.Format("File name: " + stream.Address + "\n");
                    FileEntryString += String.Format("Resource ID: " + stream.ResourceId + "\n");
                    FileEntryString += String.Format("Stream ID: " + stream.StreamId + "\n");
                    FileEntryString += String.Format("Stream Name: " + stream.StreamName + "\n");
                    FileEntryString += String.Format("Transport Name: " + stream.TransportName + "\n");
                    FileEntryString += String.Format("Stream Codec: " + stream.CodecName + "\n");
                    FileEntryString += String.Format("User Tag: " + stream.UserTag + "\n");
                    FileEntryString += String.Format("Device Serial number: " + stream.DeviceSerialNumber + "\n");

                }

                //No need to configure measurements or decode when using direct but we still need the list of measurements with stream id
                
                measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Nano"));
                measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Ax"));
                measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Ay"));
                measurements.Add(new OxTS.NavLib.Common.Measurement.MeasurementItem("Az"));


                current_time = file_ds_manager.GetStoreStartTime();
                end_time = file_ds_manager.GetStoreEndTime();

                //Make sure not to enter this section again
                Setup = true;
                OpenFilePlusSetup();
            }
            else
            {
                //Tell the data store to seek to the correct position
                file_ds_manager.BeginDirectDecode(current_time);
                Int32 k = 0;
                //Counts through the data fo 0.05s
                while (k < 5)
                {

                    List<MeasurementValue> meas_data = file_ds_manager.GetDirectStreamDataForInstant(current_time, streams[0].StreamId, measurements);
                    OutputNanoString = String.Format(meas_data.Find(a => a.MeasurementName == "Nano").ValueString);
                    OutputAxString = String.Format(meas_data.Find(a => a.MeasurementName == "Ax").ValueString);
                    OutputAyString = String.Format(meas_data.Find(a => a.MeasurementName == "Ay").ValueString);
                    OutputAzString = String.Format(meas_data.Find(a => a.MeasurementName == "Az").ValueString);
                    // increment time
                    current_time = current_time + 10000000;
                    k++;
                }

                i++;

                //close handles to stream data
                file_ds_manager.EndDirectDecode();
            }
        }
    } 
}
