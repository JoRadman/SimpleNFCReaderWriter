using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleNFCReaderWriter
{
    public partial class Write : Form
    {
        Timer time = new Timer();

        DataTable globalTable = new DataTable();

        DataColumn No = new DataColumn("No", typeof(string));
        DataColumn DataType = new DataColumn("DataType", typeof(string));
        DataColumn DataLenght = new DataColumn("DataLenght", typeof(string));
        DataColumn Data = new DataColumn("Data", typeof(string));

        string assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public const int BLUETOOTH_ADDRESS_SIZE_WITH_DELIMITERS = 17;
        private const string URI_IDENTIFIER_CODE_TAG_OPEN = "<#";
        private const string URI_IDENTIFIER_CODE_TAG_CLOSE = ">";
        private readonly int URI_IDENTIFIER_CODE_TAG_LEN = URI_IDENTIFIER_CODE_TAG_OPEN.Length + URI_IDENTIFIER_CODE_TAG_CLOSE.Length;

        public Write()
        {
            InitializeComponent();
        }

        private void ReadDllVerziju()
        {
            uint dll_ver = 0;
            bool tryDefaultDllPath = false;
            bool reportDllError = false;

            //-------------------------------------------------------
            // uFR DLL
#if WIN64
            string DllPath = "..\\..\\..\\lib\\windows\\x86_64"; // for x64 target
#else
            string DllPath = "..\\..\\..\\lib\\windows\\x86"; // for x86 target
#endif
            string path = Directory.GetCurrentDirectory();
            string assemblyProbeDirectory = DllPath;
            try
            {
                Directory.SetCurrentDirectory(assemblyProbeDirectory);
                dll_ver = uFCoderMulti.GetDllVersion();
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                tryDefaultDllPath = true;
            }
            catch (System.DllNotFoundException)
            {
                tryDefaultDllPath = true;
            }
            catch (System.BadImageFormatException)
            {
                tryDefaultDllPath = true;
            }
            Directory.SetCurrentDirectory(path);
            if (tryDefaultDllPath)
            {
                try
                {
                    dll_ver = uFCoderMulti.GetDllVersion();
                }
                catch (System.DllNotFoundException)
                {
                    reportDllError = true;
                }
                catch (System.BadImageFormatException)
                {
                    reportDllError = true;
                }
                if (reportDllError)
                {
                    MessageBox.Show("Error while importing uFCoder library.\n" +
                        "Can't find dll file or library file is corrupted",
                        "Dll import error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }
        private DL_STATUS OpenReader()
        {
            DL_STATUS status;

            uint reader_type;
            byte[] reader_sn = new byte[8];
            byte fw_major_ver;
            byte fw_minor_ver;
            byte fw_build;
            byte hw_major;
            byte hw_minor;

            //-------------------------------------------------------
            status = uFCoderMulti.ReaderOpen();
            if (status != DL_STATUS.UFR_OK)
            {
                return status;
            }

            //-------------------------------------------------------
            unsafe
            {
                fixed (byte* f_rdsn = reader_sn)
                    status = uFCoderMulti.GetReaderSerialDescription(f_rdsn);
            }

            unsafe
            {
                status |= uFCoderMulti.GetReaderType(&reader_type);

                status |= uFCoderMulti.GetReaderHardwareVersion(&hw_major, &hw_minor);

                status |= uFCoderMulti.GetReaderFirmwareVersion(&fw_major_ver, &fw_minor_ver);

                status |= uFCoderMulti.GetBuildNumber(&fw_build);
            }

            if (status != DL_STATUS.UFR_OK)
            {
                return status;
            }

            return DL_STATUS.UFR_OK;
        }

        private DLOGIC_CARD_TYPE GetCardType()
        {
            DL_STATUS status;
            byte cardtype_val = 0;
            DLOGIC_CARD_TYPE cardtype;

            unsafe
            {
                status = uFCoderMulti.GetDlogicCardType(&cardtype_val);
            }

            if (status != DL_STATUS.UFR_OK)
            {
                cardtype_val = 0;
            }

            cardtype = (DLOGIC_CARD_TYPE)cardtype_val;

            return cardtype;
        }

        private int getcardlen(DLOGIC_CARD_TYPE cardtype)
        {
            int data_len;

            switch (cardtype)
            {
                case DLOGIC_CARD_TYPE.DL_MIFARE_MINI:
                    data_len = 320;
                    break;
                case DLOGIC_CARD_TYPE.DL_MIFARE_CLASSIC_1K:
                    data_len = 752;
                    break;
                case DLOGIC_CARD_TYPE.DL_MIFARE_CLASSIC_4K:
                    data_len = 3440;
                    break;
                case DLOGIC_CARD_TYPE.DL_NTAG_203:
                    data_len = 144;
                    break;
                case DLOGIC_CARD_TYPE.DL_NTAG_213:
                    data_len = 144;
                    break;
                case DLOGIC_CARD_TYPE.DL_NTAG_215:
                    data_len = 504;
                    break;
                case DLOGIC_CARD_TYPE.DL_NTAG_216:
                    data_len = 888;
                    break;
                case DLOGIC_CARD_TYPE.DL_MIFARE_ULTRALIGHT:
                    data_len = 48;
                    break;
                case DLOGIC_CARD_TYPE.DL_MIFARE_ULTRALIGHT_C:
                    data_len = 144;
                    break;
                case DLOGIC_CARD_TYPE.DL_MIFARE_ULTRALIGHT_EV1_11:
                    data_len = 48;
                    break;
                case DLOGIC_CARD_TYPE.DL_MIFARE_ULTRALIGHT_EV1_21:
                    data_len = 144;
                    break;
                case DLOGIC_CARD_TYPE.DL_NT3H_1101:
                case DLOGIC_CARD_TYPE.DL_NT3H_2111:
                case DLOGIC_CARD_TYPE.DL_NT3H_2211:
                    data_len = 888;
                    break;
                case DLOGIC_CARD_TYPE.DL_NT3H_1201:
                    data_len = 1904;
                    break;
                case DLOGIC_CARD_TYPE.DL_UNKNOWN_ISO_14443_4:
                    data_len = 8192;
                    break;
                default:
                    data_len = 0;
                    break;
            }
            return data_len;
        }
        private byte[] SubByteArray(byte[] sourceArray, int out_len)
        {
            byte[] truncArray = new byte[out_len];
            try
            {
                Array.Copy(sourceArray, truncArray, truncArray.Length);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " ::::::::::: \n" + ex.StackTrace);
            }
            return truncArray;
        }

        private void ReadCard()
        {
            DL_STATUS result = DL_STATUS.UNKNOWN_ERROR;

            try
            {
                byte[] type = new byte[256];
                byte[] id = new byte[256];
                byte[] payload = new byte[1000];
                byte type_length, id_length, tnf;
                byte record_nr;
                byte message_cnt, record_cnt, empty_record_cnt;
                byte[] record_cnt_array = new byte[100];
                DLOGIC_CARD_TYPE cardtype;
                string ct;
                int card_len;

                cardtype = GetCardType();

                ct = String.Format("[{0:X}]", (int)cardtype);
                ct += " " + cardtype.ToString();

                card_len = getcardlen(cardtype);


                if (cardtype == DLOGIC_CARD_TYPE.DL_NO_CARD)
                {
                    return;
                }
                unsafe
                {
                    fixed (byte* pData = record_cnt_array)
                        result = uFCoderMulti.get_ndef_record_count(&message_cnt, &record_cnt, pData, &empty_record_cnt);
                }

                if (result != DL_STATUS.UFR_OK)
                {
                    return;
                }
               // gc_NFCPisi.DataSource = null;

                uint payload_length;

                for (record_nr = 1; record_nr < record_cnt + 1; record_nr++)
                {
                    unsafe
                    {
                        fixed (byte* f_type = type)
                        fixed (byte* f_id = id)
                        fixed (byte* f_payload = payload)

                            result = uFCoderMulti.read_ndef_record(1, (byte)record_nr, &tnf, f_type, &type_length, f_id, &id_length, f_payload, &payload_length);
                    }


                    if (result != DL_STATUS.UFR_OK)
                    {
                        if (result == DL_STATUS.UFR_WRONG_NDEF_CARD_FORMAT)
                            MessageBox.Show(" NDEF format error");
                        else if (result == DL_STATUS.UFR_NDEF_MESSAGE_NOT_FOUND)
                            MessageBox.Show(" NDEF message not found");
                        else
                            MessageBox.Show(" Error: " + result);

                        break;
                    }

                    string str_payload = System.Text.Encoding.UTF8.GetString(SubByteArray(payload, (int)payload_length));
                    string str_type = System.Text.Encoding.UTF8.GetString(SubByteArray(type, (int)type_length));
                    string str_tnf = "TNF: " + System.Convert.ToString(tnf);
                    string str_id = System.Text.Encoding.UTF8.GetString(SubByteArray(id, (int)id_length));
                    //---------------------------------------------------------------------------

                    string[] row = { record_nr.ToString(), str_type.ToString(), payload_length.ToString(), str_payload };

                    //DataRow red = globalTable.NewRow();

                    tb_Data.Text = str_payload;

                    //globalTable.Rows.Add(red);

                    //gc_NFCPisi.DataSource = globalTablica;
                    //gv_NFCPisi.BestFitColumns();
                }

                //gc_NFCPisi.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " ::::::::::: \n" + ex.StackTrace);
            }
        }
        private DL_STATUS ndef_write(int TNF, string Type, string ID, byte[] Payload)
        {
            DL_STATUS result = DL_STATUS.UNKNOWN_ERROR;

            try
            {
                byte card_formated;

                byte tnf = (byte)TNF;

                byte[] type = System.Text.Encoding.UTF8.GetBytes(Type);
                byte type_length = (byte)type.Length;

                byte[] id = System.Text.Encoding.UTF8.GetBytes(ID);
                byte id_length = (byte)ID.Length;

                byte[] payload = Payload;
                uint payload_length = (uint)payload.Length;

                unsafe
                {
                    fixed (byte* f_type = type)
                    fixed (byte* f_id = id)
                    fixed (byte* f_payload = payload)
                        result = uFCoderMulti.write_ndef_record(1, &tnf, f_type, &type_length, f_id, &id_length, f_payload, &payload_length, &card_formated);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " ::::::::::: \n" + ex.StackTrace);
            }
            return result;
        }

        private void WriteOnTheCard()
        {
            try
            {
                DL_STATUS result = DL_STATUS.UNKNOWN_ERROR;

                int tnf;
                string type;
                string id;
                byte[] payload;
                string tmp_str = "";
                byte[] tmp_payload;

                string Data;

                Data = tb_Data.Text.Trim();

                if (Data.Length == 0)
                {
                    MessageBox.Show("Please write the data you want to write on the card.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb_Data.Focus();
                    return;
                }
                if (tb_Data.Text != string.Empty)
                {
                    MessageBox.Show("There is already data written on this card.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                tmp_str = Data;

                tmp_payload = System.Text.Encoding.UTF8.GetBytes(tmp_str);

                payload = tmp_payload;

                tnf = 2;
                type = "ID";
                id = "";

                result = ndef_write(tnf, type, id, payload);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " ::::::::::: \n" + ex.StackTrace);
            }
        }

        private void Ocisti()
        {
            //gc_NFCPisi.DataSource = null;
            //te_Zapisi.Text = string.Empty;
            //globalTablica.Clear();
        }

        //private void dodajKolone()
        //{
        //    globalTablica.Columns.Add(RedniBroj);
        //    globalTablica.Columns.Add(TipPodatka);
        //    globalTablica.Columns.Add(DuzinaPodatka);
        //    globalTablica.Columns.Add(Podatak);
        //}
        private void WriteMethod()
        {
            try
            {
                if (cb_Connection.Checked == false)
                {
                    MessageBox.Show("Please check connection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (tb_Data.Text == string.Empty)
                {
                    MessageBox.Show("Please write the data you want to write on the card.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb_Data.Focus();
                    return;
                }

                DLOGIC_CARD_TYPE CardType;
                CardType = GetCardType();

                if (CardType == DLOGIC_CARD_TYPE.DL_NO_CARD)
                {
                    MessageBox.Show("Please put the card on the reader.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (tb_Data.Text != string.Empty)
                {
                    MessageBox.Show("There is already data written on this card.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    WriteOnTheCard();
                    uFCoderMulti.ReaderUISignal(3, 3);
                    ReadCard();


                    if (tb_Data.Text == string.Empty)
                    {
                        WriteOnTheCard();
                        ReadCard();
                    }

                    if (tb_Data.Text == string.Empty)
                    {
                        MessageBox.Show("It's not possible to write on the card. Authorisation mistake. Please use other card and try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    MessageBox.Show("You wrote data succesfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tb_Data.Text = string.Empty;
                    tb_Data.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " ::::::::::: \n" + ex.StackTrace);
            }
            ReadCard();
        }

        private void Read()
        {
            if (cb_Connection.Checked == false)
            {
                MessageBox.Show("Please check connection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DLOGIC_CARD_TYPE cardType;
            cardType = GetCardType();

            if (cardType == DLOGIC_CARD_TYPE.DL_NO_CARD)
            {
                MessageBox.Show("Please put the card on the reader.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            else
            {
                uFCoderMulti.ReaderUISignal(1, 1);
                ReadCard();
            }
        }

        private void cb_Connection_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_Connection.Checked == true)
            {
                OpenReader();
            }
            if (cb_Connection.Checked == false)
            {
                uFCoderMulti.ReaderClose();
            }
        }

        private void Write_Load(object sender, EventArgs e)
        {
            ReadDllVerziju();
            OpenReader();
        }

        private void btn_readCard_Click(object sender, EventArgs e)
        {
            Read();
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            Read();
            WriteMethod();
        }
    }
}
