using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

namespace MG_BlocksEngine2.Serializer
{
    public static class BE2_BlockXML
    {
        // write
        public static XElement SBlockToXElement(BE2_SerializableBlock serializableBlock)
        {
            XElement xBlock = new XElement("Block");

            xBlock.Add(new XElement("blockName", serializableBlock.blockName));
            xBlock.Add(new XElement("position", serializableBlock.position));
            // v2.9 - isVariable flag (made obsolete) replaced by varManagerName field on the BE2_SerializableBlock to enable and facilitate the addition of custom variable types
            xBlock.Add(new XElement("varManagerName", serializableBlock.varManagerName));
            xBlock.Add(new XElement("varName", serializableBlock.varName));
            XElement xSections = new XElement("sections");
            xBlock.Add(xSections);
            foreach (BE2_SerializableSection section in serializableBlock.sections)
            {
                XElement xSection = SSectionToXElement(section);
                xSections.Add(xSection);
            }

            return xBlock;
        }

        public static XElement SSectionToXElement(BE2_SerializableSection serializableSection)
        {
            XElement xSection = new XElement("Section");

            XElement xChildBlocks = new XElement("childBlocks");
            xSection.Add(xChildBlocks);
            foreach (BE2_SerializableBlock sChildBlock in serializableSection.childBlocks)
            {
                XElement xChildBlock = SBlockToXElement(sChildBlock);
                xChildBlocks.Add(xChildBlock);
            }

            XElement xInputs = new XElement("inputs");
            xSection.Add(xInputs);
            foreach (BE2_SerializableInput sInput in serializableSection.inputs)
            {
                XElement xInput = SInputToXElement(sInput);
                xInputs.Add(xInput);
            }

            return xSection;
        }

        public static XElement SInputToXElement(BE2_SerializableInput serializableInput)
        {
            XElement xInput = new XElement("Input");

            xInput.Add(new XElement("isOperation", serializableInput.isOperation));
            xInput.Add(new XElement("value", serializableInput.value));
            if (serializableInput.isOperation)
            {
                XElement xOperation = new XElement("operation");
                xInput.Add(xOperation);
                XElement xOperationBlock = SBlockToXElement(serializableInput.operation);
                xOperation.Add(xOperationBlock);
            }

            return xInput;
        }

        // read
        public static BE2_SerializableBlock XElementToSBlock(XElement xBlock)
        {
            BE2_SerializableBlock sBlock = new BE2_SerializableBlock();
            sBlock.blockName = xBlock.Element("blockName").Value;
            sBlock.position = BE2_BlockXMLUtils.StringToVector3(xBlock.Element("position").Value);
            // v2.9 - isVariable flag (made obsolete) replaced by varManagerName field on the BE2_SerializableBlock to enable and facilitate the addition of custom variable types
            sBlock.varManagerName = xBlock.Element("varManagerName")?.Value;
            sBlock.varName = xBlock.Element("varName").Value;
            IEnumerable<XElement> xSections = xBlock.Element("sections").Elements("Section");
            foreach (XElement xSection in xSections)
            {
                BE2_SerializableSection sSection = XElementToSSection(xSection);
                sBlock.sections.Add(sSection);
            }

            return sBlock;
        }

        public static BE2_SerializableSection XElementToSSection(XElement xSection)
        {
            BE2_SerializableSection sSection = new BE2_SerializableSection();

            IEnumerable<XElement> xChildBlocks = xSection.Element("childBlocks").Elements("Block");
            foreach (XElement xChildBlock in xChildBlocks)
            {
                BE2_SerializableBlock sChildBlock = XElementToSBlock(xChildBlock);
                sSection.childBlocks.Add(sChildBlock);
            }

            IEnumerable<XElement> xInputs = xSection.Element("inputs").Elements("Input");
            foreach (XElement xInput in xInputs)
            {
                BE2_SerializableInput sInput = XElementToSInput(xInput);
                sSection.inputs.Add(sInput);
            }

            return sSection;
        }

        public static BE2_SerializableInput XElementToSInput(XElement xInput)
        {
            BE2_SerializableInput sInput = new BE2_SerializableInput();

            sInput.isOperation = xInput.Element("isOperation").Value == "true";
            sInput.value = xInput.Element("value").Value;
            if (sInput.isOperation)
            {
                BE2_SerializableBlock sOperationBlock = XElementToSBlock(xInput.Element("operation").Element("Block"));
                sInput.operation = sOperationBlock;
            }

            return sInput;
        }

    }

    public static class BE2_BlockXMLUtils
    {
        public static Vector3 StringToVector3(string stringValue)
        {
            Vector3 vectorValue = Vector3.zero;

            string[] xyz = stringValue.TrimStart('(').TrimEnd(')').Split(',');
            vectorValue.x = StringToFloat(xyz[0]);
            vectorValue.y = StringToFloat(xyz[1]);
            vectorValue.z = StringToFloat(xyz[2]);

            return vectorValue;
        }

        public static float StringToFloat(string stringValue)
        {
            float floatValue = 0;
            try
            {
                floatValue = float.Parse(stringValue, CultureInfo.InvariantCulture);
                return floatValue;
            }
            catch
            {
                return 0;
            }
        }
    }
}
