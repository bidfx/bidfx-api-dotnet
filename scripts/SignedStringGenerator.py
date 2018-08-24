def GenerateString(limit, username, level):
    if level != 1 && level != 2:
        return "Error: Level is not 1 or 2"
    if level == 1 && limit <= 20:
        print("Warning: the limit is less than the default (20)")
    elif level == 2 && limit <= 500:
        print("Warning: the limit is less than the default (500)")
    hash = GetHash(limit, username, level)
    return str(limit) + "-" + username + "-" + str(level) + "-" + SignPart(hash)

def GetHash(limit, username, level):
    hash = limit
    hash = int32(int32(hash * 31) + level)
    hash = int32(int32(hash * 31) + HashString(username))
    if hash < 0:
        if hash == -2147483648:
            return 0
        return -hash
    return hash
    
def SignPart(part):
    return str(pow(part, 75507918513594124599288698597218005281, 125134336105432108045835366424157859929))

def HashString(input):
    h = 0
    for c in input:
        h = h * 31 + ord(c)
    return int32(h)

def int32(x):
  x = x & 0xFFFFFFFF
  if x>0x7FFFFFFF:
    x=int(0x100000000-x)
    if x<2147483648:
      return -x
    else:
      return -2147483648
  return x
