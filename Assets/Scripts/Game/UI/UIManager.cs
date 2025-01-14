using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gfen.Game.UI
{
    public class UIManager : MonoBehaviour
    {
        public UIPage[] pages;

        private GameManager m_gameManager;

        private Dictionary<Type, UIPage> m_typePageDict = new Dictionary<Type, UIPage>();

        private Stack<UIPage> m_pageStack = new Stack<UIPage>();

        public void Init(GameManager gameManager)
        {
            m_gameManager = gameManager;

            foreach (var page in pages)
            {
                var pageType = page.GetType();
                m_typePageDict[pageType] = page;
            }
        }

        public T ShowPage<T>() where T : UIPage
        {
            var pageType = typeof(T);
            var page = m_typePageDict[pageType];

            if (m_pageStack.Count > 0)
            {
                var topPage = m_pageStack.Peek();
                if (topPage != null)
                {
                    topPage.Hide();
                }
            }

            m_pageStack.Push(page);
            page.canvas.sortingOrder = m_pageStack.Count*10;

            page.Show(m_gameManager);

            return page as T;
        }

        public void HidePage()
        {
            if (m_pageStack.Count > 0)
            {
                var page = m_pageStack.Pop();

                page.Hide();
            }

            if (m_pageStack.Count > 0)
            {
                var topPage = m_pageStack.Peek();
                if (topPage != null)
                {
                    topPage.Show(m_gameManager);
                }
            }
        }

        public void HideAllPages()
        {
            while (m_pageStack.Count > 0)
            {
                var page = m_pageStack.Pop();

                page.Hide();
            }
        }
    }
}
