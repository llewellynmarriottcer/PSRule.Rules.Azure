private static JToken UniqueString(
      string function,
      JToken[] parameters,
      TemplateErrorAdditionalInfo additionalInfo)
    {
      ExpressionBuiltInFunctions.ValidateParametersAtLeastOne(function, parameters, additionalInfo);
      string str = ((IEnumerable<JToken>) parameters).All<JToken>((Func<JToken, bool>) (parameter => parameter.IsTextBasedJTokenType())) ? 
      ((IEnumerable<JToken>) parameters).Select<JToken, string>((Func<JToken, string>) (parameter => parameter.ToObject<string>())).ConcatStrings("-") : 
      throw new ExpressionException(ErrorResponseMessages.InvalidTemplateFunctionParametersStringLiterals.ToLocalizedMessage((object) function), additionalInfo, (Exception) null);
      // concat args with '-' character

      // string must be less than 131072 characters long
      return str.Length <= 131072 ? 
      (JToken) ExpressionBuiltInFunctions.Base32Encode(ComputeHash.MurmurHash64(str)) : 
      throw new ExpressionException(ErrorResponseMessages.TemplateLiteralLimitExceeded.ToLocalizedMessage((object) 131072, (object) str.Length), additionalInfo, (Exception) null);
    }


  private static string Base32Encode(ulong input)
    {
      string str = "abcdefghijklmnopqrstuvwxyz234567";
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < 13; ++index)
      {
        stringBuilder.Append(str[(int) (input >> 59)]);
        input <<= 5;
      }
      return stringBuilder.ToString();
    }

    public static ulong MurmurHash64(string str, uint seed = 0) => ComputeHash.MurmurHash64(Encoding.UTF8.GetBytes(str), seed);


    public static ulong MurmurHash64(byte[] data, uint seed = 0)
    {
      int length = data.Length;
      uint num1 = seed;
      uint num2 = seed;
      int index;
      for (index = 0; index + 7 < length; index += 8)
      {
        uint num3 = (uint) ((int) data[index] | (int) data[index + 1] << 8 | (int) data[index + 2] << 16 | (int) data[index + 3] << 24);
        uint num4 = (uint) ((int) data[index + 4] | (int) data[index + 5] << 8 | (int) data[index + 6] << 16 | (int) data[index + 7] << 24);
        uint num5 = (num3 * 597399067U).RotateLeft32(15) * 2869860233U;
        num1 = (uint) ((int) ((num1 ^ num5).RotateLeft32(19) + num2) * 5 + 1444728091);
        uint num6 = (num4 * 2869860233U).RotateLeft32(17) * 597399067U;
        num2 = (uint) ((int) ((num2 ^ num6).RotateLeft32(13) + num1) * 5 + 197830471);
      }
      int num7 = length - index;
      if (num7 > 0)
      {
        int num8;
        if (num7 < 4)
        {
          switch (num7)
          {
            case 2:
              num8 = (int) data[index] | (int) data[index + 1] << 8;
              break;
            case 3:
              num8 = (int) data[index] | (int) data[index + 1] << 8 | (int) data[index + 2] << 16;
              break;
            default:
              num8 = (int) data[index];
              break;
          }
        }
        else
          num8 = (int) data[index] | (int) data[index + 1] << 8 | (int) data[index + 2] << 16 | (int) data[index + 3] << 24;
        uint num9 = ((uint) num8 * 597399067U).RotateLeft32(15) * 2869860233U;
        num1 ^= num9;
        if (num7 > 4)
        {
          int num10;
          switch (num7)
          {
            case 6:
              num10 = (int) data[index + 4] | (int) data[index + 5] << 8;
              break;
            case 7:
              num10 = (int) data[index + 4] | (int) data[index + 5] << 8 | (int) data[index + 6] << 16;
              break;
            default:
              num10 = (int) data[index + 4];
              break;
          }
          uint num11 = ((uint) num10 * 2869860233U).RotateLeft32(17) * 597399067U;
          num2 ^= num11;
        }
      }
      uint num12 = num1 ^ (uint) length;
      uint num13 = num2 ^ (uint) length;
      uint num14 = num12 + num13;
      uint num15 = num13 + num14;
      uint num16 = (num14 ^ num14 >> 16) * 2246822507U;
      uint num17 = (num16 ^ num16 >> 13) * 3266489909U;
      uint num18 = num17 ^ num17 >> 16;
      uint num19 = (num15 ^ num15 >> 16) * 2246822507U;
      uint num20 = (num19 ^ num19 >> 13) * 3266489909U;
      uint num21 = num20 ^ num20 >> 16;
      uint num22 = num18 + num21;
      return (ulong) (num21 + num22) << 32 | (ulong) num22;
    }