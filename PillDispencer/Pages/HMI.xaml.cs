﻿
using ControlzEx.Standard;
using Org.BouncyCastle.Ocsp;
using Ozeki;
using PillDispencer.Model;
using PillDispencer.PopUp;
using PillDispencer.Services;
using PillDispencer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using utils;

namespace PillDispencer.Pages
{
    /// <summary>
    /// Interaction logic for HMI.xaml
    /// </summary>
    public partial class HMI : Page
    {
        BrushConverter bc;
        CommunicationDevices weighing;
        CommunicationDevices control;
        private Dispatcher _dispathcer;

        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }
        HMIViewModel hMIViewModel;
        private DeviceInfo deviceInfo;
        System.Timers.Timer ExecutionTimer = new System.Timers.Timer();


        #region function Variable
        const int REC_BUF_SIZE = 10000;
        byte[] recBuf = new byte[REC_BUF_SIZE];
        byte[] recBufParse = new byte[REC_BUF_SIZE];

        public Byte[] _MbTgmBytes;
        internal UInt32 TotalReceiveSize = 0;
        private int readIndex = 0;
        #endregion

        private readonly string sessionId;

        public HMI()
        {
            bc = new BrushConverter();
            hMIViewModel = new HMIViewModel();
            InitializeComponent();
            this.DataContext = hMIViewModel;
            _dispathcer = Dispatcher.CurrentDispatcher;
            sessionId = "HMISession_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            weighing = new CommunicationDevices
            {
                IsConfigured = false
            };
            control = new CommunicationDevices
            {
                IsConfigured = false
            };
            deviceInfo = DeviceInformation.GetConnectedDevices();

            LoadSytem();
        }


        #region page function
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Starting Next batch..");
            StartBatch();
        }

        private void SaveWeight_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is HMIViewModel model)
            {
                model.ActualWeight = model.Weight;
                model.SetPoint0Percent = 100;
                model.SetPoint0 = hMIViewModel.ActualWeight;
            }
            MessageBox.Show("Weight has been set");
        }

        private void ResetWeight_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is HMIViewModel model)
            {
                model.Weight = 0;
                model.WeightPercentage = 0;
                MessageBox.Show("Weight has been reset");
            }
        }

        private void SaveTareWeight_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TareWeightText.Text)) return;


            if (this.DataContext is HMIViewModel model)
            {
                if (Regex.IsMatch(TareWeightText.Text.ToString(), @"^(\+|-)?\d+(\.\d+)?$"))
                {
                    model.TareWeight = Math.Round(Convert.ToDecimal(TareWeightText.Text.ToString()), 2);
                    if (model.ActualWeight > 0)
                    {
                        model.ActualWeight = model.ActualWeight - model.TareWeight;
                    }

                    MessageBox.Show("Tare Weight has been set");
                }
                else
                {
                    MessageBox.Show("Incorrect input provided.");
                    TareWeightText.Text = "";
                }

            }

        }

        private void ResetTareWeight_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is HMIViewModel model)
            {
                model.TareWeight = 0;
                MessageBox.Show("Tare Weight has been reset");
            }
        }

        private void SaveCalibration_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Calibration has been set");
        }

        private void ResetCalibration_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is HMIViewModel model)
            {
                model.Zero = 0;
                model.Span = 0;
                model.Factor = 0;
                model.CalculateSpan = true;
                model.ActualWeight = 0;
                model.Weight = 0;
                model.WeightPercentage = 0;
                MessageBox.Show("Calibration has been reset");


            }
        }

        #endregion


        #region Run function
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DataContext is HMIViewModel model)
                {
                    if (model.ActualWeight == 0 || model.Zero == 0 || model.Span == 0)
                    {
                        MessageBox.Show("Please set up weight, tare weight and other configurations.");
                        return;
                    }
                }

                if (!weighing.IsConfigured || !control.IsConfigured)
                {
                    MessageBox.Show("Please configure Devices.");
                    LoadSytem();
                }
                if (this.DataContext is HMIViewModel hmodel)
                {
                    hmodel.IsNotRunning = false;
                }
                if (!weighing.IsConfigured || !control.IsConfigured)
                {
                    MessageBox.Show("You have not configured devices.");
                    return;
                }
                //MessageBox.Show("Starting process..");
                _dispathcer.Invoke(new Action(() =>
                {
                    MessageLog.Text = "Starting process..";
                }));
                ExecuteLogic();
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                weighing.IsTurnedOn = false;
                control.IsTurnedOn = false;

                StopPortCommunication((int)Model.Module_Device_Type.UART);
                StopPortCommunication((int)Model.Module_Device_Type.ModBus);
                if (this.DataContext is HMIViewModel model)
                {
                    model.IsNotRunning = true;
                    model.IsRunning = false;
                    model.Weight = 0;
                    model.WeightPercentage = 0;
                    model.IsSetPoint0Passed = false;
                    model.IsSetPoint1Passed = false;
                    model.IsSetPoint2Passed = false;
                    model.IsBatchComplete = false;

                    WriteControCardState(control.Green.RegisterNo, 0, control.SlaveAddress);
                    WriteControCardState(control.Yellow.RegisterNo, 0, control.SlaveAddress);
                    WriteControCardState(control.Red.RegisterNo, 0, control.SlaveAddress);
                    _dispathcer.Invoke(new Action(() =>
                    {
                        red.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                        red.Background = (Brush)bc.ConvertFromString("#cecece");
                        red.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                        red.IsChecked = false;
                        TextRed.Content = "OFF";

                        Green.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                        Green.Background = (Brush)bc.ConvertFromString("#cecece");
                        Green.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                        Green.IsChecked = false;
                        TextGreen.Content = "OFF";

                        yellow.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                        yellow.Background = (Brush)bc.ConvertFromString("#cecece");
                        yellow.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                        yellow.IsChecked = false;
                        TextYellow.Content = "OFF";
                        MessageLog.Text = "Processing Stopped.";
                    }));

                    return;
                }
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
        }

        private void LoadSytem()
        {
            if (!weighing.IsConfigured)
            {
                ConfigurationDailog dailog = new ConfigurationDailog("Set Weight Device Configuration", deviceInfo.CustomDeviceInfos);
                dailog.ShowDialog();
                if (!dailog.Canceled)
                {

                    weighing.PortName = deviceInfo.CustomDeviceInfos.Where(x => x.DeviceID == dailog.DeviceId).FirstOrDefault().PortName;
                    weighing.DeviceId = dailog.DeviceId;
                    weighing.BaudRate = dailog.BaudRate;
                    weighing.DataBit = dailog.Databit;
                    weighing.StopBit = dailog.Stopbit;
                    weighing.Parity = dailog.ParityValue;
                    weighing.IsConfigured = true;
                }
            }

            if (!control.IsConfigured)
            {
                ConfigurationDailog dailog = new ConfigurationDailog("Set Control Card Configuration", deviceInfo.CustomDeviceInfos);
                dailog.ShowDialog();
                if (!dailog.Canceled)
                {

                    control.PortName = deviceInfo.CustomDeviceInfos.Where(x => x.DeviceID == dailog.DeviceId).FirstOrDefault().PortName;
                    control.DeviceId = dailog.DeviceId;
                    control.BaudRate = dailog.BaudRate;
                    control.DataBit = dailog.Databit;
                    control.StopBit = dailog.Stopbit;
                    control.Parity = dailog.ParityValue;

                    DeviceInformationPopUp deviceInformation = new DeviceInformationPopUp();
                    deviceInformation.ShowDialog();
                    if (!deviceInformation.Canceled)
                    {
                        control.SlaveAddress = Convert.ToInt32(deviceInformation.AddressBox.Text.ToString());
                        control.Green = new RegisterConfiguration
                        {
                            RType = 1,
                            RegisterNo = Convert.ToInt32(deviceInformation.GreenLight.Text.ToString())
                        };
                        control.Yellow = new RegisterConfiguration
                        {
                            RType = 1,
                            RegisterNo = Convert.ToInt32(deviceInformation.YellowLight.Text.ToString())
                        };
                        control.Red = new RegisterConfiguration
                        {
                            RType = 1,
                            RegisterNo = Convert.ToInt32(deviceInformation.RedLight.Text.ToString())
                        };
                        control.IsConfigured = true;
                    }
                }
                else return;
            }

            ConnectWeight(weighing.PortName, weighing.BaudRate, weighing.DataBit, weighing.StopBit, weighing.Parity);
        }

        private void ExecuteLogic()
        {
            hMIViewModel.IsNotRunning = false;
            hMIViewModel.IsRunning = true;
            hMIViewModel.Weight = 0;
            hMIViewModel.WeightPercentage = 0;
            hMIViewModel.IsSetPoint0Passed = false;
            hMIViewModel.IsSetPoint1Passed = false;
            hMIViewModel.IsSetPoint2Passed = false;
            hMIViewModel.IsBatchComplete = false;
            ConnectWeight(weighing.PortName, weighing.BaudRate, weighing.DataBit, weighing.StopBit, weighing.Parity);
            Connect_control_card(control.PortName, control.BaudRate, control.DataBit, control.StopBit, control.Parity);

            hMIViewModel.IsSetPoint0Passed = true;
            WriteControCardState(control.Green.RegisterNo, 1, control.SlaveAddress);
            WriteControCardState(control.Yellow.RegisterNo, 0, control.SlaveAddress);
            WriteControCardState(control.Red.RegisterNo, 0, control.SlaveAddress);

            _dispathcer.Invoke(new Action(() =>
            {
                MessageLog.Text = "Batch Started..";
                red.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                red.Background = (Brush)bc.ConvertFromString("#cecece");
                red.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                TextRed.Content = "OFF";

                Green.Foreground = Brushes.Green;
                Green.Background = Brushes.LightGreen;
                Green.BorderBrush = Brushes.Green;
                Green.IsChecked = true;
                TextGreen.Content = "ON";

                yellow.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                yellow.Background = (Brush)bc.ConvertFromString("#cecece");
                yellow.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                TextYellow.Content = "OFF";
            }));

        }
        #endregion

        #region Serial Port Coms

        private void SerialPortCommunications(ref SerialPort serialPort, int TypeOfDevice = 0, string port = "", int baudRate = 0, int databit = 0, int stopBit = 0, int parity = 0)
        {

            try
            {
                serialPort = new SerialPort(port);
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = databit;
                serialPort.StopBits = stopBit == 0 ? StopBits.None : (stopBit == 1 ? StopBits.One : (stopBit == 2 ? StopBits.Two : StopBits.OnePointFive));
                switch (parity)
                {
                    case (int)Parity.None:
                        serialPort.Parity = Parity.None;
                        break;
                    case (int)Parity.Odd:
                        serialPort.Parity = Parity.Odd;
                        break;
                    case (int)Parity.Even:
                        serialPort.Parity = Parity.Even;
                        break;
                    case (int)Parity.Mark:
                        serialPort.Parity = Parity.Mark;
                        break;
                    case (int)Parity.Space:
                        serialPort.Parity = Parity.Space;
                        break;
                }

                serialPort.Handshake = Handshake.None;
                serialPort.Encoding = ASCIIEncoding.ASCII;
                switch (TypeOfDevice)
                {
                    case (int)Model.Module_Device_Type.UART:
                        serialPort.DataReceived += WeightDevice_DataReceived;
                        //serialPort.ReadTimeout= 1000;
                        break;
                    case (int)Model.Module_Device_Type.ModBus:
                        serialPort.DataReceived += ControlDevice_DataReceived;
                        break;
                }


                serialPort.Open();
            }
            catch
            {

            }

        }

        private void StopPortCommunication(int device)
        {
            if (weighing.SerialDevice != null && device == (int)Model.Module_Device_Type.UART)
            {
                if (weighing.SerialDevice.IsOpen)
                {
                    weighing.SerialDevice.DtrEnable = false;
                    weighing.SerialDevice.RtsEnable = false;
                    weighing.SerialDevice.DataReceived -= WeightDevice_DataReceived;
                    weighing.SerialDevice.DiscardInBuffer();
                    weighing.SerialDevice.DiscardOutBuffer();
                    weighing.SerialDevice.Close();
                }
            }
            else if (control.SerialDevice != null && device == (int)Model.Module_Device_Type.ModBus)
            {
                if (control.SerialDevice.IsOpen)
                {
                    control.SerialDevice.DtrEnable = false;
                    control.SerialDevice.RtsEnable = false;
                    control.SerialDevice.DataReceived -= ControlDevice_DataReceived;
                    control.SerialDevice.DiscardInBuffer();
                    control.SerialDevice.DiscardOutBuffer();
                    control.SerialDevice.Close();
                }
            }

        }
        #endregion


        #region logic
        private void ConnectWeight(string Port, int Baudrate, int databit, int stopbit, int parity)
        {
            try
            {
                if (weighing.IsTurnedOn == false)
                {

                    weighing.RecState = 1;
                    RecData _recData = new RecData();
                    _recData.SessionId = Common.GetSessionNewId;
                    _recData.Ch = 0;
                    _recData.Indx = 0;
                    _recData.Reg = 0;
                    _recData.NoOfVal = 0;
                    _recData.Status = PortDataStatus.Requested;
                    weighing.CurrentRequest = _recData;
                    SerialPort WeightDevice = new SerialPort();
                    SerialPortCommunications(ref WeightDevice, (int)Model.Module_Device_Type.UART, Port, Baudrate, databit, stopbit, parity);
                    weighing.SerialDevice = WeightDevice;
                    weighing.IsTurnedOn = true;
                }
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
        }

        private void Connect_control_card(string Port, int Baudrate, int databit, int stopbit, int parity)
        {

            try
            {

                if (control.IsTurnedOn == false)
                {

                    control.RecState = 1;
                    RecData _recData = new RecData();
                    _recData.SessionId = Common.GetSessionNewId;
                    _recData.Ch = 0;
                    _recData.Indx = 0;
                    _recData.Reg = 0;
                    _recData.NoOfVal = 0;
                    _recData.Status = PortDataStatus.Requested;
                    _recData.RqType = (int)Model.Module_Device_Type.ModBus;
                    control.CurrentRequest = _recData;
                    control.RecState = 0;
                    SerialPort WeightDevice = new SerialPort();
                    SerialPortCommunications(ref WeightDevice, (int)Model.Module_Device_Type.ModBus, Port, Baudrate, databit, stopbit, parity);
                    control.SerialDevice = WeightDevice;

                }
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
        }

        private void WeightDevice_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {


            if (weighing.IsTurnedOn == false)
            {
                return;
            }

            switch (weighing.RecState)
            {
                case 0:
                    break;
                case 1:
                    try
                    {
                        
                        //int i = 0;
                        weighing.RecIdx = 0;
                        weighing.RecState = 1;
                        recBuf = new byte[weighing.SerialDevice.BytesToRead];
                        //if(recBuf.Length<=30)
                        //{
                        //    recBuf = new byte[REC_BUF_SIZE];
                        //    return;
                        //}
                        weighing.SerialDevice.Read(recBuf, 0, recBuf.Length);
                        weighing.ReceiveBufferQueue = new Queue<byte[]>();
                        weighing.ReceiveBufferQueue.Enqueue(recBuf);
                        weighing.LastResponseReceived = System.DateTime.Now;
                        weighing.IsComplete = true;
                        recBuf = new byte[REC_BUF_SIZE];
                        UartDataReader();
                    }
                    catch (Exception ex)
                    {
                        string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                        log = log + "\r\n error description : " + ex.ToString();
                        LogWriter.LogWrite(log, sessionId);
                    }

                    break;
            }



        }

        private void ControlDevice_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string log = "Response received from control card.";
            LogWriter.LogWrite(log, sessionId);

            if (control.IsTurnedOn == false)
            {
                return;
            }
            switch (control.RecState)
            {
                case 0:
                    break;
                case 1:
                    try
                    {
                        int i = 0;
                        control.RecIdx = 0;
                        control.RecState = 2;


                        while (control.SerialDevice.BytesToRead > 0)
                        {
                            byte[] rec = new byte[1];
                            control.RecIdx += control.SerialDevice.Read(rec, 0, 1);
                            recBuf[i] = rec[0];
                            i++;
                        }

                        if (control.RecIdx > 3)
                        {
                            TotalReceiveSize = (uint)recBuf[2] + 5;
                        }
                        if (TotalReceiveSize > control.RecIdx)
                        {
                            control.IsComplete = false;
                        }
                        else
                        {
                            control.IsComplete = true;
                            control.ReceiveBufferQueue = new Queue<byte[]>();
                            control.ReceiveBufferQueue.Enqueue(recBuf);
                            recBuf = new byte[REC_BUF_SIZE];
                        }
                        control.LastResponseReceived = System.DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                        log = log + "\r\n error description : " + ex.ToString();
                        LogWriter.LogWrite(log, sessionId);
                    }
                    break;
            }

            ControlDataReader();
        }

        void ReadControlCardResponse(RecData _recData)
        {
            try
            {
                if (_recData.MbTgm != null)
                {
                    if (_recData.MbTgm.Length > 0 && _recData.MbTgm.Length > readIndex)
                    {
                        //To Read Function Code response.
                        if (_recData.MbTgm[1] == (int)COM_Code.three)
                        {

                            int _i0 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 3);
                            int _i1 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 5);
                            int _i2 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 7);
                            int _i3 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 9);
                            int _i4 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 11);
                            int _i5 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 13);
                            int _i6 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 15);
                            int _i7 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 17);
                            int _i8 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 19);
                            int _i9 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 21);
                            int _i10 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 23);
                            int _i11 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 25);
                            int _i12 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 27);
                            int _i13 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 29);
                            int _i14 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 31);
                            int _i15 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 33);
                            int _i16 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 35);
                            int _i17 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 37);
                            int _i18 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 39);
                            int _i19 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 41);
                            int _i20 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 43);
                            int _i21 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 45);
                            int _i22 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 47);
                            int _i23 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 49);
                            int _i24 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 51);
                            int _i25 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 53);
                            int _i26 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 55);
                            int _i27 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 57);
                            int _i28 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 59);
                            int _i29 = ByteArrayConvert.ToUInt16(Common.MbTgmBytes, 61);

                            // write logic here..

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }

        }

        private void WriteControCardState(int reg, int val, int slave)
        {
            string log = "Sending request with value " + val + " to control card on register " + reg + " and add address slave";
            LogWriter.LogWrite(log, sessionId);
            try
            {
                control.RecState = 1;
                RecData _recData = new RecData();
                _recData.SessionId = Common.GetSessionNewId;
                _recData.Ch = 0;
                _recData.Indx = 0;
                _recData.Reg = 0;
                _recData.NoOfVal = 0;
                _recData.Status = PortDataStatus.Requested;
                control.CurrentRequest = _recData;
                control.IsComplete = false;

                MODBUSComnn obj = new MODBUSComnn();
                Common.COMSelected = COMType.MODBUS;
                int[] _val = new int[2] { 0, val };
                obj.SetMultiSendorValueFM16(slave, 0, control.SerialDevice, reg + 1, 1, "ControlCard", 1, 0, Model.DeviceType.ControlCard, _val, false);   // GetSoftwareVersion(Common.Address, Common.Parity, sp, _ValueType);
            }
            catch (Exception ex)
            {
                log = "An error has occured while sending request to control card:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
        }

        private void ReadAllControCardInputOutput()
        {
            try
            {
                control.RecState = 1;
                RecData _recData = new RecData();
                _recData.SessionId = Common.GetSessionNewId;
                _recData.Ch = 0;
                _recData.Indx = 0;
                _recData.Reg = 0;
                _recData.NoOfVal = 0;
                _recData.Status = PortDataStatus.Requested;
                control.CurrentRequest = _recData;
                control.IsComplete = false;

                MODBUSComnn obj = new MODBUSComnn();
                obj.GetMultiSendorValueFM3(control.SlaveAddress, 0, control.SerialDevice, 0, 30, "ControlCard", 1, 0, Model.DeviceType.ControlCard);
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
        }

        private bool CheckTgmError(RecData _recData, Byte[] _payLoad, Byte[] _MBTgm, int _MbLength)
        {
            bool _IsTgmErr = false;
            if (_recData != null)
            {
                switch (_recData.RqType)
                {
                    case RQType.ModBus:
                        ModBus_Ack _Ack = MbTgm.GetModBusAck(_payLoad);
                        bool _IsOk = CrcModbus.CheckCrc(_MBTgm, _MbLength);
                        if (!_IsOk)
                        {
                            _Ack = ModBus_Ack.CrcFault;
                        }

                        if (_Ack == ModBus_Ack.OK)  // MobBus TgmCRC Check
                        {
                            //if (_recData.deviceType != Models.DeviceType.MotorDerive)
                            //{
                            _IsTgmErr = true;
                            //ToolTipStatus = ComStatus.OK;
                            Common.GoodTmgm++;
                            //}
                        }
                        else if (_Ack == ModBus_Ack.CrcFault)
                        {
                            _IsTgmErr = false;
                            //ToolTipStatus = ComStatus.CRC;
                            Common.CRCFaults++;
                        }
                        else if (_Ack == ModBus_Ack.Timeout)
                        {
                            _IsTgmErr = false;
                            //ToolTipStatus = ComStatus.TimeOut;
                            Common.TimeOuts++;
                        }
                        else
                        {
                            _IsTgmErr = false;
                            //ToolTipStatus = ComStatus.CRC;
                            Common.CRCFaults++;
                        }
                        break;

                }
            }
            return _IsTgmErr;
        }

        private void UartDataReader()
        {
            try
            {
                if (weighing.RecState > 0 && weighing.IsComplete)
                {
                    weighing.IsComplete = false;


                    //recState = 1;
                    while (weighing.ReceiveBufferQueue.Count > 0)
                    {
                        recBufParse = weighing.ReceiveBufferQueue.Dequeue();

                        weighing.RecState = 1;
                        RecData _recData = weighing.CurrentRequest;

                        if (_recData != null)
                        {

                            Common.GoodTmgm++;

                            weighing.CurrentRequest.MbTgm = recBufParse;
                            weighing.CurrentRequest.Status = PortDataStatus.Received;
                            CompareWeightModuleResponse(weighing.CurrentRequest);
                            return;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
        }

        void CompareWeightModuleResponse(RecData _recData)
        {
            try
            {
                string log = string.Empty;
                _dispathcer.Invoke(new Action(() =>
                {
                    MessageLog.Text = "Reading weight..";

                }));

                log = "Reading weight..";
                LogWriter.LogWrite(log, sessionId);
                if (_recData.MbTgm.Length > 0 && _recData.MbTgm.Length > readIndex)
                {
                    byte[] bytestToRead = _recData.MbTgm.Skip(readIndex).ToArray();
                    string str = Encoding.Default.GetString(bytestToRead).Replace(System.Environment.NewLine, string.Empty);

                    string actualdata = Regex.Replace(str, @"[^\t\r\n -~]", "_").RemoveWhitespace().Trim();
                    if (string.IsNullOrWhiteSpace(actualdata)) return;

                    log = "Received raw data : " + actualdata;
                    LogWriter.LogWrite(log, sessionId);

                    //if (actualdata.StartsWith("_") || actualdata.EndsWith("_"))
                    //{
                    //    actualdata = filterString(actualdata);
                    //}

                    //if (!actualdata.EndsWith("m"))
                    //{
                    //    actualdata = actualdata.EndsWith("m") == false ? actualdata.Remove(actualdata.Length - 1, 1) : actualdata;
                    //    actualdata = filterString(actualdata);
                    //}


                    string[] data = actualdata.Split('_');

                    var lastitem = data[data.Length - 1];
                    var outP = lastitem.ToLower().ToString();




                    if (!string.IsNullOrEmpty(outP))
                    {
                        Regex re = new Regex(@"\d+");
                        Match m = re.Match(outP);
                        decimal balance = Convert.ToDecimal(m.Value);
                        log = "Filtered raw data : " + balance;
                        LogWriter.LogWrite(log, sessionId);


                        if (hMIViewModel.Zero <= 0 && hMIViewModel.IsNotRunning == true)
                        {
                            log = "Zero value has been set to " + balance;
                            LogWriter.LogWrite(log, sessionId);
                            hMIViewModel.Zero = balance;
                            _dispathcer.Invoke(new Action(() =>
                            {
                                MessageLog.Text = "Zero value has been set, Now put some weight on the scale and enter the actual weight in span to set calibration factor.";
                            }));
                            return;
                        }

                        if (hMIViewModel.Span <= 0)
                        {
                            log = "Span has not been set";
                            LogWriter.LogWrite(log, sessionId);
                            _dispathcer.Invoke(new Action(() =>
                            {
                                MessageLog.Text = "Please put some weight on the scale and set span value for calibrations calculation...";
                            }));
                            return;
                        }

                        if (hMIViewModel.CalculateSpan == true && hMIViewModel.IsNotRunning == true && hMIViewModel.Span > 0)
                        {
                            decimal diff = balance - hMIViewModel.Zero;
                            decimal divident = diff / hMIViewModel.Span;
                            hMIViewModel.Factor = Math.Round(divident, 2);
                            hMIViewModel.CalculateSpan = false;
                            _dispathcer.Invoke(new Action(() =>
                            {
                                MessageLog.Text = "Calibration completed, Please set tare weight if needed...";
                            }));

                            log = "Calibration has been set to " + hMIViewModel.Factor + ", Please set tare weight if needed....";
                            LogWriter.LogWrite(log, sessionId);
                            //hMIViewModel.Weight = 0;
                            return;
                        }
                        if (hMIViewModel.Factor <= 0)
                        {
                            log = "Calibration factor is zero.";
                            LogWriter.LogWrite(log, sessionId);
                            _dispathcer.Invoke(new Action(() =>
                            {
                                MessageLog.Text = "calibrations is zero, Please enter some weight to calculate calibration.";
                            }));
                            return;
                        }

                        decimal dweight = Convert.ToDecimal(hMIViewModel.Zero * 20) / 100;
                        decimal plus = hMIViewModel.Zero + dweight;
                        decimal minus = hMIViewModel.Zero - dweight;
                        if ((balance <= plus || balance >= minus) && hMIViewModel.AutoZeroEnabled == true && hMIViewModel.Factor > 0)
                        {
                            hMIViewModel.Weight = 0;
                            hMIViewModel.ActualWeight = 0;
                            hMIViewModel.WeightPercentage = 0;
                            return;
                        }

                        decimal weight = balance - hMIViewModel.Zero;
                        weight = weight / hMIViewModel.Factor;
                        weight = Math.Round(weight, 2);

                        if (hMIViewModel.IsNotRunning == true && hMIViewModel.ActualWeight <= 0)
                        {
                            hMIViewModel.Weight = weight;
                            log = "Actual weight has not been set.";
                            LogWriter.LogWrite(log, sessionId);
                            _dispathcer.Invoke(new Action(() =>
                            {
                                MessageLog.Text = "Please Set 100% weight.";
                            }));
                            return;
                        }

                        weight = weight - hMIViewModel.TareWeight;
                        hMIViewModel.Weight = Math.Round(weight, 2);
                        hMIViewModel.WeightPercentage = Math.Round((hMIViewModel.Weight / hMIViewModel.ActualWeight) * 100, 2);
                        if (hMIViewModel.ActualWeight > 0 && hMIViewModel.IsNotRunning == true)
                        {
                            log = "Weight has been set. Actual Weight is : " + hMIViewModel.ActualWeight;
                            LogWriter.LogWrite(log, sessionId);

                            if (hMIViewModel.SetPoint1Percent <= 0 || hMIViewModel.SetPoint2Percent <= 0)
                            {
                                log = "Set Points not checked..";
                                LogWriter.LogWrite(log, sessionId);
                                _dispathcer.Invoke(new Action(() =>
                                {
                                    MessageLog.Text = "Please check set points or enter custom set points";

                                }));
                                return;
                            }

                            return;
                        }




                        if (hMIViewModel.IsNotRunning == false)
                        {
                            if (hMIViewModel.SetPoint1Percent > 0 && hMIViewModel.SetPoint2Percent > 0)
                            {
                                hMIViewModel.SetPoint1 = Math.Round((Convert.ToDecimal(hMIViewModel.SetPoint1Percent) / 100) * hMIViewModel.ActualWeight, 2);
                                hMIViewModel.SetPoint2 = Math.Round((Convert.ToDecimal(hMIViewModel.SetPoint2Percent) / 100) * hMIViewModel.ActualWeight, 2);
                                log = "Set point 1 is : " + hMIViewModel.SetPoint1 + ", Set point 2 is " + hMIViewModel.SetPoint2;
                                LogWriter.LogWrite(log, sessionId);
                            }

                            if (weight > hMIViewModel.SetPoint1 && weight <= hMIViewModel.SetPoint0 && hMIViewModel.IsSetPoint0Passed == false)
                            {
                                _dispathcer.Invoke(new Action(() =>
                                {
                                    MessageLog.Text = "Weight is decreasing from 100%..";
                                }));
                                hMIViewModel.IsSetPoint0Passed = true;
                                WriteControCardState(control.Green.RegisterNo, 1, control.SlaveAddress);
                                WriteControCardState(control.Yellow.RegisterNo, 0, control.SlaveAddress);
                                WriteControCardState(control.Red.RegisterNo, 0, control.SlaveAddress);

                                _dispathcer.Invoke(new Action(() =>
                                {
                                    red.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                                    red.Background = (Brush)bc.ConvertFromString("#cecece");
                                    red.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                                    TextRed.Content = "OFF";

                                    Green.Foreground = Brushes.Green;
                                    Green.Background = Brushes.LightGreen;
                                    Green.BorderBrush = Brushes.Green;
                                    Green.IsChecked = true;
                                    TextGreen.Content = "ON";

                                    yellow.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                                    yellow.Background = (Brush)bc.ConvertFromString("#cecece");
                                    yellow.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                                    TextYellow.Content = "OFF";

                                }));

                                log = "Weight is decreasing from 100%. Current Weight is : " + weight + ", Actual Weight is :" + hMIViewModel.ActualWeight;
                                LogWriter.LogWrite(log, sessionId);

                                return;
                            }

                            if (weight > hMIViewModel.SetPoint2 && weight <= hMIViewModel.SetPoint1 && hMIViewModel.IsSetPoint1Passed == false)
                            {
                                hMIViewModel.IsSetPoint0Passed = true;
                                hMIViewModel.IsSetPoint1Passed = true;
                                WriteControCardState(control.Green.RegisterNo, 0, control.SlaveAddress);
                                WriteControCardState(control.Yellow.RegisterNo, 1, control.SlaveAddress);
                                WriteControCardState(control.Red.RegisterNo, 0, control.SlaveAddress);
                                log = "Weight is decreasing from " + hMIViewModel.SetPoint1Percent + "%. Current Weight is : " + weight + ", Actual Weight is :" + hMIViewModel.ActualWeight; ;
                                LogWriter.LogWrite(log, sessionId);
                                _dispathcer.Invoke(new Action(() =>
                                    {
                                        red.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                                        red.Background = (Brush)bc.ConvertFromString("#cecece");
                                        red.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                                        TextRed.Content = "OFF";

                                        Green.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                                        Green.Background = (Brush)bc.ConvertFromString("#cecece");
                                        Green.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                                        TextGreen.Content = "OFF";

                                        yellow.Foreground = Brushes.Orange;
                                        yellow.Background = Brushes.LightYellow;
                                        yellow.BorderBrush = Brushes.Yellow;
                                        yellow.IsChecked = true;
                                        TextYellow.Content = "ON";
                                        MessageLog.Text = "Weight has passed from set point 1.";

                                    }));
                                return;
                            }

                            if (weight <= hMIViewModel.SetPoint2 && hMIViewModel.IsSetPoint2Passed == false)
                            {
                                hMIViewModel.IsSetPoint0Passed = true;
                                hMIViewModel.IsSetPoint1Passed = true;
                                hMIViewModel.IsSetPoint2Passed = true;
                                WriteControCardState(control.Green.RegisterNo, 0, control.SlaveAddress);
                                WriteControCardState(control.Yellow.RegisterNo, 0, control.SlaveAddress);
                                WriteControCardState(control.Red.RegisterNo, 1, control.SlaveAddress);

                                log = "Weight is decreasing from " + hMIViewModel.SetPoint2Percent + "%. Current Weight is : " + hMIViewModel.Weight;
                                LogWriter.LogWrite(log, sessionId);

                                _dispathcer.Invoke(new Action(() =>
                                    {
                                        red.Foreground = Brushes.Red;
                                        red.Background = Brushes.LightGoldenrodYellow;
                                        red.BorderBrush = Brushes.Red;
                                        red.IsChecked = true;
                                        TextRed.Content = "ON";

                                        Green.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                                        Green.Background = (Brush)bc.ConvertFromString("#cecece");
                                        Green.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                                        TextGreen.Content = "OFF";

                                        yellow.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                                        yellow.Background = (Brush)bc.ConvertFromString("#cecece");
                                        yellow.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                                        TextYellow.Content = "OFF";
                                    }));
                                return;
                            }
                        }

                        if (hMIViewModel.IsSetPoint0Passed && hMIViewModel.IsSetPoint1Passed && hMIViewModel.IsSetPoint2Passed)
                        {
                            log = "BATCH COMPLETED. Current Weight is : " + hMIViewModel.Weight;
                            LogWriter.LogWrite(log, sessionId);
                            _dispathcer.Invoke(new Action(() =>
                            {
                                MessageLog.Text = "BATCH COMPLETED..";
                            }));
                            BatchCompleted();
                        }
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }


        }

        public string filterString(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return filter;
            while (filter.StartsWith("_") || filter.EndsWith("_"))
            {
                filter = filter.StartsWith("_") == true ? filter.Substring(1) : filter;
                filter = filter.EndsWith("_") == true ? filter.Remove(filter.Length - 1, 1) : filter;
            }
            return filter;
        }

        void BatchCompleted()
        {
            try
            {
                weighing.IsTurnedOn = false;
                control.IsTurnedOn = false;
                StopPortCommunication((int)Model.Module_Device_Type.UART);
                StopPortCommunication((int)Model.Module_Device_Type.ModBus);

                hMIViewModel.Weight = 0;
                hMIViewModel.WeightPercentage = 0;
                hMIViewModel.IsNotRunning = true;
                hMIViewModel.IsRunning = false;
                hMIViewModel.IsSetPoint0Passed = false;
                hMIViewModel.IsSetPoint1Passed = false;
                hMIViewModel.IsSetPoint2Passed = false;
                hMIViewModel.IsBatchComplete = true;
                _dispathcer.Invoke(new Action(() =>
                {
                    MessageLog.Text = "Batch has been completed.";
                }));
            }
            catch (Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }

        }
        void StartBatch()
        {
            hMIViewModel.IsNotRunning = false;
            hMIViewModel.IsRunning = true;
            hMIViewModel.Weight = 0;
            hMIViewModel.WeightPercentage = 0;
            hMIViewModel.IsSetPoint0Passed = false;
            hMIViewModel.IsSetPoint1Passed = false;
            hMIViewModel.IsSetPoint2Passed = false;
            hMIViewModel.IsBatchComplete = false;

            ConnectWeight(weighing.PortName, weighing.BaudRate, weighing.DataBit, weighing.StopBit, weighing.Parity);
            Connect_control_card(control.PortName, control.BaudRate, control.DataBit, control.StopBit, control.Parity);


            hMIViewModel.IsSetPoint0Passed = true;
            WriteControCardState(control.Green.RegisterNo, 1, control.SlaveAddress);
            WriteControCardState(control.Yellow.RegisterNo, 0, control.SlaveAddress);
            WriteControCardState(control.Red.RegisterNo, 0, control.SlaveAddress);

            _dispathcer.Invoke(new Action(() =>
            {
                MessageLog.Text = "Starting new batch..";
                red.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                red.Background = (Brush)bc.ConvertFromString("#cecece");
                red.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                TextRed.Content = "OFF";

                Green.Foreground = Brushes.Green;
                Green.Background = Brushes.LightGreen;
                Green.BorderBrush = Brushes.Green;
                Green.IsChecked = true;
                TextGreen.Content = "ON";

                yellow.Foreground = (Brush)bc.ConvertFromString("#b8b8b8");
                yellow.Background = (Brush)bc.ConvertFromString("#cecece");
                yellow.BorderBrush = (Brush)bc.ConvertFromString("#e6e6e6");
                TextYellow.Content = "OFF";

            }));
        }

        private void ControlDataReader()
        {

            if (control.RecState > 0 && control.IsComplete)
            {
                control.IsComplete = false;


                //recState = 1;
                while (control.ReceiveBufferQueue.Count > 0)
                {
                    recBufParse = control.ReceiveBufferQueue.Dequeue();

                    control.RecState = 1;
                    SB1Reply _reply = new SB1Reply(Common.GetSessionId);
                    SB1Handler _hndl = new SB1Handler(control.SerialDevice);
                    UInt32 length = 32;
                    UInt32 payLoadSize = 0;
                    Byte[] payload;
                    Byte[] RxSB1 = recBufParse;
                    Byte MbAck;
                    UInt16 MbLength;
                    UInt16 Reserved;
                    Byte[] mbTgmBytes;


                    length = (uint)recBufParse[2] + 5;
                    payLoadSize = 0;
                    RxSB1 = recBufParse;
                    RecData _recData = control.CurrentRequest;

                    //if (_reply.CheckCrc(recBufParse, Convert.ToInt32(_reply.length)))  // SB1 Check CRC
                    //{

                    if (_recData != null)
                    {
                        //ExtractPayload
                        Byte[] _payload = new Byte[1000];
                        Array.Copy(RxSB1, 30, _payload, 0, payLoadSize);
                        payload = _payload;

                        //Set payloadrs
                        MbAck = (Byte)payload[0];
                        MbLength = PillDispencer.Model.ByteArrayConvert.ToUInt16(payload, 1);
                        Reserved = PillDispencer.Model.ByteArrayConvert.ToUInt16(payload, 3);


                        //extract modbus tgm
                        Byte[] _MbTgm = new Byte[1000];
                        Array.Copy(_payload, 5, _MbTgm, 0, MbLength);
                        mbTgmBytes = _MbTgm;


                        _MbTgmBytes = RxSB1;
                        MbLength = (ushort)_reply.length;
                        if (_MbTgmBytes != null && MbLength > 0)
                        {
                            bool _IsTgmErr = false;
                            _IsTgmErr = CheckTgmError(_recData, _payload, _MbTgmBytes, MbLength);
                            if (_IsTgmErr)
                            {
                                if (_recData.RqType == RQType.WireLess)
                                {
                                    mbTgmBytes = payload;
                                }
                                else
                                {
                                    mbTgmBytes = _MbTgmBytes;
                                }

                                control.CurrentRequest.MbTgm = mbTgmBytes;
                                control.CurrentRequest.Status = PortDataStatus.Received;
                                ReadControlCardResponse(control.CurrentRequest);
                                return;

                            }
                        }

                    }
                    else
                    {
                        if (_recData != null)
                        {
                            _recData.Status = PortDataStatus.Normal;
                            if (_recData.SessionId > 0)
                            {

                                control.CurrentRequest = _recData;
                                ReadControlCardResponse(control.CurrentRequest);
                            }

                        }
                    }


                }

            }

        }


        #endregion

        private void Span_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textb = sender as System.Windows.Controls.TextBox;

            if (!string.IsNullOrEmpty(textb.Text.ToString()))
            {
                hMIViewModel.Span = Convert.ToDecimal(textb.Text.ToString());
                hMIViewModel.CalculateSpan = true;
            }

        }

        private void WeightCheckCustomValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = sender as System.Windows.Controls.TextBox;
            if (!string.IsNullOrEmpty(text.Text) && CustomCheck.IsChecked == true)
            {

                hMIViewModel.SetPoint1Percent = Math.Round(Convert.ToDecimal(text.Text.ToString()), 2);

            }
        }

        private void ControlCheckCustomValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = sender as System.Windows.Controls.TextBox;
            if (!string.IsNullOrEmpty(text.Text) && CustomControlCheck.IsChecked == true)
            {
                hMIViewModel.SetPoint2Percent = Math.Round(Convert.ToDecimal(text.Text.ToString()), 2);
            }
        }

        private void SetPoint1Check_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio.IsChecked == true)
            {

                if (radio.Tag.ToString() == "Custom")
                {
                    if (!string.IsNullOrEmpty(WeightCheckCustomValue.Text.ToString()))
                    {
                        if (Regex.IsMatch(WeightCheckCustomValue.Text.ToString(), @"^\d+$"))
                        {
                            hMIViewModel.SetPoint1Percent = Math.Round(Convert.ToDecimal(WeightCheckCustomValue.Text.ToString()), 2);
                        }
                    }
                }
                else
                {
                    hMIViewModel.SetPoint1Percent = Convert.ToDecimal(radio.Tag.ToString());
                }

            }

        }
        private void SetPoint2Check_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio.IsChecked == true)
            {

                if (radio.Tag.ToString() == "Custom")
                {
                    if (!string.IsNullOrEmpty(ControlCheckCustomValue.Text.ToString()))
                    {
                        if (Regex.IsMatch(ControlCheckCustomValue.Text.ToString(), @"^\d+$"))
                        {
                            hMIViewModel.SetPoint2Percent = Math.Round(Convert.ToDecimal(ControlCheckCustomValue.Text.ToString()), 2);
                        }

                    }
                }
                else
                {
                    hMIViewModel.SetPoint2Percent = Convert.ToDecimal(radio.Tag.ToString());
                }

            }
        }


    }
}
