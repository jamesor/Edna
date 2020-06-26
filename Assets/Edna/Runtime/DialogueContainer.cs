﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace JamesOR.Edna.Dialogue
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        //public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        //public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();
    }
}
