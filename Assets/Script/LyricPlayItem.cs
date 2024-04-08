using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ver2
{
    public class LyricPlayItem : MonoBehaviour
    {
        [HideInInspector] public LyricPlayUI lyricPlayUI;
        [SerializeField] AudioSource audioSource;

        [SerializeField] RectTransform bar;
        [SerializeField] RectTransform phonetics;
        [SerializeField] Text content;
        [SerializeField] RectTransform contentWidth;

        List<CheckPoint> checkPoints;
        int curMaxCheckIndex;
        float disppearTime;

        int nextCheckIndex;
        float checkStartTime;
        float checkEndTime;
        float checkPos;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            LyricPlay();

            if (audioSource.time >= disppearTime) Clear();
        }

        //ÿ֡���ã��ƶ���ʽ�����
        void LyricPlay()
        {
            if (checkStartTime <= audioSource.time && nextCheckIndex < curMaxCheckIndex)
            {
                bar.DOAnchorPos(new Vector2(checkPos * 1.33f - 1000 + contentWidth.rect.width / 2, 0), checkEndTime - checkStartTime);
                nextCheckIndex++;
                SetNextCheck();
            }
        }

        //��ȡ�����Ϣ
        public void SetLyric(Sentence sentence)
        {
            Clear();
            content.text = sentence.content;
            bar.gameObject.GetComponent<Image>().color = sentence.color;

            foreach (Phonetic phonetic in sentence.phonetics)
            {
                PhoneticText temp = lyricPlayUI.poolContent.GetChild(0).GetComponent<PhoneticText>();
                temp.Init(phonetic.content, phonetic.pos * 1.33f, sentence.color, phonetics);
            }

            checkPoints = sentence.checkPoints;
            curMaxCheckIndex = checkPoints.Count;
            disppearTime = checkPoints[curMaxCheckIndex - 1].endTime / 1000 + 1;

            nextCheckIndex = 0;
            SetNextCheck();
        }

        //���ݵ�ǰ��checkPoints��nextCheckIndexȷ����һ��check�Ľ���ʱ��
        void SetNextCheck()
        {
            if (nextCheckIndex >= curMaxCheckIndex) return;

            checkStartTime = checkPoints[nextCheckIndex].startTime / 1000f - 0.5f;
            if (nextCheckIndex + 1 < curMaxCheckIndex)
            {
                //����Ϊ�������һ��check�������ʱ��Ϊ��һ��check�Ŀ�ʼʱ��
                checkEndTime = checkPoints[nextCheckIndex + 1].startTime / 1000f - 0.5f;
            }
            else
            {
                //��Ϊ�������һ��check�������ʱ��Ϊ��check����
                checkEndTime = checkPoints[nextCheckIndex].endTime / 1000f - 0.5f;
            }
            checkPos = checkPoints[nextCheckIndex].pos;
        }

        //������ݺ�ע������ʼ�����λ��
        public void Clear()
        {
            content.text = "";
            bar.anchoredPosition = new Vector2(-1000, 0);
            int phoneticCount = phonetics.childCount;
            for (int j = 0; j < phoneticCount; j++)
            {
                phonetics.GetChild(0).GetComponent<PhoneticText>().Clear();
            }
        }
    }
}
