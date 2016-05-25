#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    fingerPrint.cs
//
// Author:      Marco D'Alessandro
// Date:        July 2014
//
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SafenetIntegration.Safenet.BusinessObject
{
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class fingerPrintDetails
  {
    public fingerPrintDetails()
    {
      this.customerList = new List<customerType>();
      this.fingerprintList = new List<fingerprintType>();
    }

    [XmlArrayItem("Fingerprint", typeof(fingerprintType), IsNullable = false)]
    public List<fingerprintType> fingerprintList { get; set; }

    [XmlArrayItem("Customer", typeof(customerType), IsNullable = false)]
    public List<customerType> customerList { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class fingerprintType
  {
    [XmlElement(DataType = "integer")]
    public string fingerprintId { get; set; }

    public string fingerprintFriendlyName { get; set; }

    public string fingerprintValue { get; set; }
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  public class customerType
  {
    public customerType()
    {
      this.fingerprintList = new List<fingerprintType>();
    }

    public string customerRefId { get; set; }

    [XmlArrayItem("Fingerprint", IsNullable = false)]
    public List<fingerprintType> fingerprintList { get; set; }
  }
}
