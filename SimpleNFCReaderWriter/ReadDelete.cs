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
    public partial class ReadDelete : Form
    {
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

        public ReadDelete()
        {
            InitializeComponent();
        }

        private void ReadDllVersion()
        {
            uint dll_ver = 0;
            bool tryDefaultDllPath = false;
            bool reportDllError = false;

            //-------------------------------------------------------
            // uFR DLL
#if WIN64
            string DllPath = "..\\..\\..\\lib\\windows\\x86_64"; // for x64 target
#else
            string DllPath = "..\\..\\..\\..\\lib\\windows\\x86"; // for x86 target
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
        private void Ocisti()
        {
            //gc_NFCCitaj.DataSource = null;
            //te_Citaj.Text = string.Empty;
            //globalTablica.Clear();
        }

        private void ReadCard()
        {
            //Ocisti();

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
                //gc_NFCCitaj.DataSource = null;

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

                    //DataRow red = globalTablica.NewRow();

                    //red["RedniBroj"] = record_nr.ToString();
                    //red["TipPodatka"] = str_type.ToString();
                    //red["DuzinaPodatka"] = str_id.ToString();
                    tb_Data.Text = str_payload;

                    //globalTablica.Rows.Add(red);

                    //gc_NFCCitaj.DataSource = globalTablica;
                    //gv_NFCCitaj.BestFitColumns();
                }

                //gc_NFCCitaj.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void dodajKolone()
        {
            //globalTablica.Columns.Add(RedniBroj);
            //globalTablica.Columns.Add(TipPodatka);
            //globalTablica.Columns.Add(DuzinaPodatka);
            //globalTablica.Columns.Add(Podatak);
        }
        public void ReadMethod()
        {
            if (cb_Connection.Checked == true)
            {
                MessageBox.Show("Please check connection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DLOGIC_CARD_TYPE tipKartice;
            tipKartice = GetCardType();

            if (tipKartice == DLOGIC_CARD_TYPE.DL_NO_CARD)
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

        private void Delete()
        {
            try
            {
                var resultt = MessageBox.Show("Are you sure you want to delete this data?", "Delete", MessageBoxButtons.OKCancel);
                if (resultt == DialogResult.OK)
                {
                    DL_STATUS status;

                    byte addressingmode;
                    byte address, address_max;
                    byte authmode;

                    byte[] sectortrailer = new byte[16] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x07, 0x80, 0x69, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
                    byte[] DEFKey = new byte[6] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
                    ushort data_len;
                    byte[] data;

                    ushort bw;

                    DLOGIC_CARD_TYPE card_type;

                    addressingmode = 0x01;
                    address = 0x00;
                    authmode = 0x61;

                    card_type = getcardtype();

                    if (card_type == DLOGIC_CARD_TYPE.DL_MIFARE_CLASSIC_1K)
                        address_max = 16;
                    else if (card_type == DLOGIC_CARD_TYPE.DL_MIFARE_CLASSIC_4K)
                        address_max = 40;
                    else
                        address_max = 0;

                    for (address = 0; address < address_max; address++)
                    {
                        unsafe
                        {
                            fixed (byte* fix_sectortrailer = sectortrailer)
                            fixed (byte* fix_DEFKey = DEFKey)
                                status = uFCoderMulti.SectorTrailerWriteUnsafe_PK(addressingmode, address, fix_sectortrailer, authmode, fix_DEFKey);
                        }
                    }

                    data_len = (ushort)getcardlen(card_type);
                    data = new byte[data_len + 1];

                    unsafe
                    {
                        if (card_type < DLOGIC_CARD_TYPE.DL_MIFARE_MINI)
                            authmode = 0x60;
                        fixed (byte* fix_data = data)
                        fixed (byte* fix_DEFKey = DEFKey)
                            status = uFCoderMulti.LinearWrite_PK(fix_data, 0, data_len, &bw, authmode, fix_DEFKey);
                    }

                    if (status == DL_STATUS.UFR_OK)
                        uFCoderMulti.ReaderUISignal(4, 4);
                    MessageBox.Show("You succesfully deleted data from the card.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ReadMethod();
        }

        private DLOGIC_CARD_TYPE getcardtype()
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

        private void ReadDelete_Load(object sender, EventArgs e)
        {
            ReadDllVersion();
            OpenReader();
        }

        private void btn_readCard_Click(object sender, EventArgs e)
        {
            ReadMethod();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void btn_Init_Click(object sender, EventArgs e)
        {
            DL_STATUS status;

            status = uFCoderMulti.ndef_card_initialization();

            MessageBox.Show("Card is ready for using.");
        }
    }
}
