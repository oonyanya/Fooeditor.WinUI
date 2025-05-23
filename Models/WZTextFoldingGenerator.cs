﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FooEditEngine;
using FooEditor;

namespace FooEditor.WinUI.Models
{
    sealed class WZTextFoldingGenerator : IFoldingStrategy
    {
        struct TextLevelInfo
        {
            public long Index;
            public int Level;
            public TextLevelInfo(long index, int level)
            {
                this.Index = index;
                this.Level = level;
            }
        }
        public IEnumerable<FoldingItem> AnalyzeDocument(Document doc, long start, long end)
        {
            Stack<TextLevelInfo> beginIndexs = new Stack<TextLevelInfo>();
            long lineHeadIndex = start;
            foreach (string lineStr in doc.GetLines(start, end))
            {
                int level = OutlineAnalyzer.GetWZTextLevel(lineStr);
                if (level != -1)
                {
                    foreach(FoldingItem item in GetFoldings(beginIndexs,level, lineHeadIndex))
                        yield return item;
                    beginIndexs.Push(new TextLevelInfo(lineHeadIndex, level));
                }
                lineHeadIndex += lineStr.Length;
            }
            foreach (FoldingItem item in GetFoldings(beginIndexs, 0, lineHeadIndex))
                yield return item;
        }

        IEnumerable<FoldingItem> GetFoldings(Stack<TextLevelInfo> beginIndexs,int level, long lineHeadIndex)
        {
            while (beginIndexs.Count > 0)
            {
                TextLevelInfo begin = beginIndexs.Peek();
                if (level > begin.Level)
                    break;
                beginIndexs.Pop();
                long endIndex = lineHeadIndex - 1;
                if (begin.Index < endIndex)
                    yield return new OutlineItem(begin.Index, endIndex,begin.Level);
            }
        }
    }
}
