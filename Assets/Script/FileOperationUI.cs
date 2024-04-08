using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ver2
{
    public class FileOperationUI : MonoBehaviour
    {
        [HideInInspector] public MainUI mainUI;

        [SerializeField] AudioSource audioSource;

        [SerializeField] Button extract;
        [SerializeField] Button checkImport;
        [SerializeField] Button checkSave;
        [SerializeField] Button loadWav;
        [SerializeField] InputField lyricInterval;
        [SerializeField] InputField workspacePath;
        Text workspacePathHoldText;
        int interval;

        // Start is called before the first frame update
        void Start()
        {
            extract.onClick.AddListener(ExtractLyric);
            checkImport.onClick.AddListener(CheckImport);
            checkSave.onClick.AddListener(CheckSave);
            loadWav.onClick.AddListener(LoadWav);

            if (!PlayerPrefs.HasKey("WorkspacePath"))
            {
                PlayerPrefs.SetString("WorkspacePath", "E:/Vedio WorkSpace/KaraLyric");
            }
            workspacePathHoldText = workspacePath.placeholder.GetComponent<Text>();
            workspacePathHoldText.text = PlayerPrefs.GetString("WorkspacePath");

            LoadWav();
            CheckImport();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S) && mainUI.isCtrlKey)
            {
                CheckSave();
            }
        }

        //以固定间隔提取歌词
        void ExtractLyric()
        {
            StreamReader reader;
            string path = workspacePath.text;
            if (path.Equals("")) path = PlayerPrefs.GetString("WorkspacePath");
            else PlayerPrefs.SetString("WorkspacePath", path);
            try
            {
                reader = new StreamReader(path + "/lyric.txt");
            }
            catch
            {
                MainUI.SetLog("路径错误");
                return;
            }

            if (!int.TryParse(lyricInterval.text, out interval))
            {
                interval = 4;
            }

            //重置歌词信息列表
            mainUI.sentenceInfos.Clear();

            //读取歌词文档并将其添加到歌词信息列表
            string temp;
            for (int i = 0; i < 200; i++)
            {
                temp = reader.ReadLine();
                if (temp == null) break;

                string[] temp1 = temp.Split('|');
                mainUI.sentenceInfos.Add(new Sentence(temp1[0]));
                for (int j = 0; j < interval - 1; j++)
                {
                    reader.ReadLine();
                }
            }
            reader.Close();

            //更新歌词显示
            mainUI.RefreshDisplay();
            MainUI.SetLog("导入成功");
        }

        //导入保存的check信息
        void CheckImport()
        {
            StreamReader reader;
            string path = workspacePath.text;
            if (path.Equals("")) path = PlayerPrefs.GetString("WorkspacePath");
            else PlayerPrefs.SetString("WorkspacePath", path);
            try
            {
                reader = new StreamReader(path + "/Check.txt");
            }
            catch
            {
                MainUI.SetLog("路径错误");
                return;
            }

            //重置歌词信息列表
            mainUI.sentenceInfos.Clear();

            //读取check文档并添加到歌词信息列表
            string temp;
            for (int i = 0; i < 200; i++)
            {
                temp = reader.ReadLine();
                if (temp == null)
                {
                    mainUI.playControlUI.SetMusicInfo("", "", "");
                    break;
                }

                if (temp.Equals("#"))
                {
                    mainUI.playControlUI.SetMusicInfo(reader.ReadLine(), reader.ReadLine(), reader.ReadLine());
                    break;
                }

                mainUI.sentenceInfos.Add((new Sentence(temp)));
            }
            reader.Close();

            //更新歌词显示
            mainUI.RefreshDisplay();
            MainUI.SetLog("导入成功");
        }

        //存储check
        public void CheckSave()
        {
            StreamWriter writer;
            string path = workspacePath.text;
            if (path.Equals("")) path = PlayerPrefs.GetString("WorkspacePath");
            else PlayerPrefs.SetString("WorkspacePath", path);
            try
            {
                writer = new StreamWriter(path + "/Check.txt");
            }
            catch
            {
                MainUI.SetLog("路径错误");
                return;
            }

            foreach (Sentence sentence in mainUI.sentenceInfos)
            {
                writer.WriteLine(sentence.ToString());
            }

            writer.WriteLine("#");
            string[] musicInfo = mainUI.playControlUI.GetMusicInfo();
            writer.WriteLine(musicInfo[0]);
            writer.WriteLine(musicInfo[1]);
            writer.WriteLine(musicInfo[2]);

            writer.Flush();
            writer.Close();
            MainUI.SetLog("存储成功");
        }


        void LoadWav()
        {
            string path = workspacePath.text;
            if (path.Equals("")) path = PlayerPrefs.GetString("WorkspacePath");
            else PlayerPrefs.SetString("WorkspacePath", path);
            try
            {
                path += "/input.wav";
                byte[] buffer = File.ReadAllBytes(path);
                audioSource.clip = LoadWavAudio(buffer, path);
            }
            catch
            {
                MainUI.SetLog("路径错误");
                return;
            }
            MainUI.SetLog("导入音频成功");
        }

        // parse wav
        AudioClip LoadWavAudio(byte[] audioBytes, string fileName)
        {
            // WAV 自定义wav解析类
            WAV wav = new WAV(audioBytes);
            AudioClip audioClip;
            if (wav.ChannelCount == 2)
            {
                audioClip = AudioClip.Create(fileName, wav.SampleCount, 2, wav.Frequency, false);
                audioClip.SetData(wav.StereoChannel, 0);
            }
            else
            {
                audioClip = AudioClip.Create(fileName, wav.SampleCount, 1, wav.Frequency, false);
                audioClip.SetData(wav.LeftChannel, 0);
            }
            return audioClip;
        }
    }


    class WAV
    {
        // convert two bytes to one float in the range -1 to 1
        static float bytesToFloat(byte firstByte, byte secondByte)
        {
            // convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);
            // convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }

        static int bytesToInt(byte[] bytes, int offset = 0)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value |= ((int)bytes[offset + i]) << (i * 8);
            }
            return value;
        }
        // properties
        public float[] LeftChannel { get; internal set; }
        public float[] RightChannel { get; internal set; }
        public float[] StereoChannel { get; internal set; }
        public int ChannelCount { get; internal set; }
        public int SampleCount { get; internal set; }
        public int Frequency { get; internal set; }


        public WAV(byte[] wav)
        {
            // Determine if mono or stereo
            ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

            // Get the frequency
            Frequency = bytesToInt(wav, 24);

            // Get past all the other sub chunks to get to the data subchunk:
            int pos = 12;   // First Subchunk ID from 12 to 16

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;

            // Pos is now positioned to start of actual sound data.
            SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
            if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

            // Allocate memory (right will be null if only mono sound)
            LeftChannel = new float[SampleCount];
            if (ChannelCount == 2) RightChannel = new float[SampleCount];
            else RightChannel = null;

            // Write to double array/s:
            int i = 0;
            while (pos < wav.Length)
            {
                LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
                if (ChannelCount == 2)
                {
                    RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                    pos += 2;
                }
                i++;
            }

            //Merge left and right channels for stereo sound
            if (ChannelCount == 2)
            {
                StereoChannel = new float[SampleCount * 2];
                //Current position in our left and right channels
                int channelPos = 0;
                //After we've changed two values for our Stereochannel, we want to increase our channelPos
                short posChange = 0;

                for (int index = 0; index < (SampleCount * 2); index++)
                {

                    if (index % 2 == 0)
                    {
                        StereoChannel[index] = LeftChannel[channelPos];
                        posChange++;
                    }
                    else
                    {
                        StereoChannel[index] = RightChannel[channelPos];
                        posChange++;
                    }
                    //Two values have been changed, so update our channelPos
                    if (posChange % 2 == 0)
                    {
                        if (channelPos < SampleCount)
                        {
                            channelPos++;
                            //Reset the counter for next iterations
                            posChange = 0;
                        }
                    }
                }
            }
            else
            {
                StereoChannel = null;
            }
        }
    }
}



