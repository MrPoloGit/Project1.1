public static string debugText = "";
public static bool SmartCursorShowing = false;
public static int SmartCursorX = -1;
public static int SmartCursorY = -1;

public static void DrawDebug(SpriteBatch sb)
{
	// Draw debug info at 500, 500 on screen.
	sb.DrawString(Main.fontMouseText, debugText, new Vector2(500f, 500f), Color.White);
}

public static void DrawSmartCursor(SpriteBatch sb)
{

    if (!ModPlayer.SmartCursorShowing || Main.player[Main.myPlayer].dead) {
        sb.DrawString(Main.fontMouseText, "", new Vector2(1000f, 500f), Color.White);
        return;
    } 
    Vector2 vector2_1 = new Vector2((float)ModPlayer.SmartCursorX, (float)ModPlayer.SmartCursorY) * 16f;
    Vector2 vector2_2 = new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
    int num1 = Main.drawToScreen ? 1 : 0;
    Vector2 position = vector2_1 - Main.screenPosition;
    if ((double)Main.player[Main.myPlayer].gravDir == -1.0)
        position.Y = (float)((double)Main.screenHeight - (double)position.Y - 16.0);
    Microsoft.Xna.Framework.Color newColor = Lighting.GetColor(ModPlayer.SmartCursorX, ModPlayer.SmartCursorY) * 1f;
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1);

    Microsoft.Xna.Framework.Graphics.Texture2D magicPixel = Main.goreTexture[Config.goreID["MagicPixel"]];

    float R = 1f;
    float G1 = 0.9f;
    float B1 = 0.1f;
    float A1 = 1f;
    float num2 = 0.6f;
    sb.Draw(magicPixel, position, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G1, B1, A1) * num2, 0.0f, Vector2.Zero, 8f, SpriteEffects.None, 0.0f);
    sb.Draw(magicPixel, position + Vector2.UnitX * 8f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G1, B1, A1) * num2, 0.0f, Vector2.Zero, 8f, SpriteEffects.None, 0.0f);
    sb.Draw(magicPixel, position + Vector2.UnitY * 8f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G1, B1, A1) * num2, 0.0f, Vector2.Zero, 8f, SpriteEffects.None, 0.0f);
    sb.Draw(magicPixel, position + Vector2.One * 8f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G1, B1, A1) * num2, 0.0f, Vector2.Zero, 8f, SpriteEffects.None, 0.0f);
    float B2 = 0.3f;
    float G2 = 0.95f;
    float num3;
    float A2 = num3 = 1f;
    sb.Draw(magicPixel, position + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G2, B2, A2) * num3, 0.0f, Vector2.Zero, new Vector2(2f, 16f), SpriteEffects.None, 0.0f);
    sb.Draw(magicPixel, position + Vector2.UnitX * 16f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G2, B2, A2) * num3, 0.0f, Vector2.Zero, new Vector2(2f, 16f), SpriteEffects.None, 0.0f);
    sb.Draw(magicPixel, position + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G2, B2, A2) * num3, 0.0f, Vector2.Zero, new Vector2(16f, 2f), SpriteEffects.None, 0.0f);
    sb.Draw(magicPixel, position + Vector2.UnitY * 16f, new Microsoft.Xna.Framework.Rectangle?(rectangle), Main.buffColor(newColor, R, G2, B2, A2) * num3, 0.0f, Vector2.Zero, new Vector2(16f, 2f), SpriteEffects.None, 0.0f);
}

public void PostDraw(Player P, SpriteBatch SB) {
    DrawSmartCursor(SB);
	DrawDebug(SB);
}

// AutoDig code

// bool trap = false;
static bool smartCursor = false;
Microsoft.Xna.Framework.Input.KeyboardState oldState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
Microsoft.Xna.Framework.Input.KeyboardState tempState;

// Helper methods

private static List<Tuple<int, int>> targets = new List<Tuple<int, int>>();
private static List<Tuple<int, int>> grappleTargets = new List<Tuple<int, int>>();
private static List<Tuple<int, int>> points = new List<Tuple<int, int>>();
private static List<Tuple<int, int>> endpoints = new List<Tuple<int, int>>();
private static List<Tuple<int, int>> toRemove = new List<Tuple<int, int>>();
private static List<Tuple<int, int>> targets2 = new List<Tuple<int, int>>();
private static short sTileHeader = (short)0;

public void UpdatePlayer(Player player)
{
    this.tempState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

    if (this.tempState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && this.oldState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.LeftControl))
    {
        smartCursor = !smartCursor;
    }

    this.oldState = this.tempState;

    SmartCursorLookup(player);
    ModPlayer.debugText = smartCursor ? "On" : "Off";

    if (player.controlLeft || player.controlRight)
    {
        if (Collision.TileCollision(player.position, new Vector2(0, 20), player.width, player.height) != player.velocity)
        {
            if (Collision.TileCollision(player.position, new Vector2(player.velocity.X, 0), player.width, player.height) != player.velocity)
            {
                if (Collision.TileCollision(player.position + new Vector2(0, -16), new Vector2(player.velocity.X, 0), player.width, player.height) == player.velocity)
                {
                    if (!player.controlDown)
                    {
                        player.position.Y -= 16;
                    }
                }
            }
        }
    }
}

#region SmartCursorLookup
public static void SmartCursorLookup(Player player)
{
    ModPlayer.debugText = "";
    //Main.SmartCursorShowing = false;
    //if (!Main.SmartCursorEnabled)
    //    return;
    SmartCursorUsageInfo providedInfo = new SmartCursorUsageInfo()
    {
        player = player,
        item = player.inventory[player.selectedItem],
        mouse = Main.MouseWorld,
        position = player.position,
        Center = player.Center
    };
    double gravDir = (double)player.gravDir;
    int tileTargetX = Player.tileTargetX;
    int tileTargetY = Player.tileTargetY;
    int tileRangeX = Player.tileRangeX;
    int tileRangeY = Player.tileRangeY;
    providedInfo.screenTargetX = Clamp<int>(tileTargetX, 10, Main.maxTilesX - 10);
    providedInfo.screenTargetY = Clamp<int>(tileTargetY, 10, Main.maxTilesY - 10);
    if (Main.tile[providedInfo.screenTargetX, providedInfo.screenTargetY] == null)
        return;
    //int num1 = SmartCursorHelper.IsHoveringOverAnInteractibleTileThatBlocksSmartCursor(providedInfo) ? 1 : 0;
    int tileBoost = 0;
    providedInfo.reachableStartX = (int)((double)player.position.X / 16.0) - tileRangeX - tileBoost + 1;
    providedInfo.reachableEndX = (int)(((double)player.position.X + (double)player.width) / 16.0) + tileRangeX + tileBoost - 1;
    providedInfo.reachableStartY = (int)((double)player.position.Y / 16.0) - tileRangeY - tileBoost + 1;
    providedInfo.reachableEndY = (int)(((double)player.position.Y + (double)player.height) / 16.0) + tileRangeY + tileBoost - 2;
    providedInfo.reachableStartX = Clamp<int>(providedInfo.reachableStartX, 10, Main.maxTilesX - 10);
    providedInfo.reachableEndX = Clamp<int>(providedInfo.reachableEndX, 10, Main.maxTilesX - 10);
    providedInfo.reachableStartY = Clamp<int>(providedInfo.reachableStartY, 10, Main.maxTilesY - 10);
    providedInfo.reachableEndY = Clamp<int>(providedInfo.reachableEndY, 10, Main.maxTilesY - 10);
    // num1 != 0

    /*if (providedInfo.screenTargetX >= providedInfo.reachableStartX && (providedInfo.screenTargetX <= providedInfo.reachableEndX && providedInfo.screenTargetY >= providedInfo.reachableStartY) && providedInfo.screenTargetY <= providedInfo.reachableEndY)
        return;
        */
    grappleTargets.Clear();
    int[] grappling = player.grappling;
    int grapCount = player.grapCount;
    for (int index = 0; index < grapCount; ++index)
    {
        Projectile projectile = Main.projectile[grappling[index]];
        int pixelCX = (int)projectile.Center.X / 16;
        int pixelCY = (int)projectile.Center.Y / 16;
        grappleTargets.Add(new Tuple<int, int>(pixelCX, pixelCY));
    }
    int num5 = -1;
    int num6 = -1;

    // Currently only using the Step_Pickaxe_MineSolids Method
    // Add if statement here for testing whether smartCursor is true?
    if (smartCursor == true)
    {
        // if(!Player.SmartCursorSettings.SmartAxeAfterPickaxe)
        // Step_Axe(providedInfo, ref num5, ref num6);
        // SmartCursorHelper.Step_ForceCursorToAnyMinableThing(providedInfo, ref num5, ref num6);
        // SmartCursorHelper.Step_Pickaxe_MineShinies(providedInfo, ref num5, ref num6);
        // -16, -10, -4, 10, 45, 204, 217, 990, 991, 992, 993
        int axe = player.inventory[player.selectedItem].axe;
        int pick = player.inventory[player.selectedItem].pick;
        int hammer = player.inventory[player.selectedItem].hammer;
        if ((pick > 0 && axe > 0) || pick > 0)
        {
            Pickaxe_MineSolids(player, providedInfo, grappleTargets, ref num5, ref num6);
        }
        else if (axe > 0)
        {
            Axe(providedInfo, ref num5, ref num6);
        }

        // if (Player.SmartCursorSettings.SmartAxeAfterPickaxe)
        if (hammer > 0) Hammers(providedInfo, ref num5, ref num6);
        // WireCutter(providedInfo, ref num5, ref num6);
        // Platforms(providedInfo, ref num5, ref num6);
        Walls(providedInfo, ref num5, ref num6);
        // Boulders(providedInfo, ref num5, ref num6);
        Torch(providedInfo, ref num5, ref num6);
        // BlocksFilling(providedInfo, ref num5, ref num6);
        // BlocksLines(providedInfo, ref num5, ref num6);
        // Acorns(providedInfo, ref num5, ref num6);
        EmptyBuckets(providedInfo, ref num5, ref num6);
        // AlchemySeeds(providedInfo, ref num5, ref num6);
        // ClayPots(providedInfo, ref num5, ref num6);
        // StaffOfRegrowth(providedInfo, ref num5, ref num6);
    }



    if (num5 != -1 && num6 != -1)
    {
        ModPlayer.SmartCursorX = Player.tileTargetX = num5;
        ModPlayer.SmartCursorY = Player.tileTargetY = num6;
        ModPlayer.debugText += "TileTargetX/Y changed";
        ModPlayer.SmartCursorShowing = true;
    }
    else
    {
        ModPlayer.debugText += "TileTargetX/Y unchanged";
        ModPlayer.SmartCursorShowing = false;
    }
}
#endregion

#region Pickaxe_MineSolids
private static void Pickaxe_MineSolids(
      Player player,
      SmartCursorUsageInfo providedInfo,
      List<Tuple<int, int>> grappleTargets,
      ref int focusedX,
      ref int focusedY)
{
    int width = player.width;
    int height = player.height;
    int direction = player.direction;
    Vector2 center = player.Center;
    Vector2 position = player.position;
    float gravDir = player.gravDir;
    int whoAmI = player.whoAmi;
    if (providedInfo.item.pick <= 0 || focusedX != -1 || focusedY != -1)
        return;

    Vector2 vector2_1 = providedInfo.mouse - center;
    int num1 = Math.Sign(vector2_1.X);
    int num2 = Math.Sign(vector2_1.Y);
    if ((double)Math.Abs(vector2_1.X) > (double)Math.Abs(vector2_1.Y) * 3.0)
    {
        num2 = 0;
        providedInfo.mouse.Y = center.Y;
    }
    if ((double)Math.Abs(vector2_1.Y) > (double)Math.Abs(vector2_1.X) * 3.0)
    {
        num1 = 0;
        providedInfo.mouse.X = center.X;
    }
    int num3 = (int)center.X / 16;
    int num4 = (int)center.Y / 16;
    points.Clear();
    endpoints.Clear();
    int num5 = 1;
    if (num2 == -1 && num1 != 0)
        num5 = -1;
    int index1 = (int)(((double)position.X + (double)(width / 2) + (double)((width / 2 - 1) * num1)) / 16.0);
    int index2 = (int)(((double)position.Y + 0.1) / 16.0);
    if (num5 == -1)
        index2 = (int)(((double)position.Y + (double)height - 1.0) / 16.0);
    int num6 = width / 16 + (width % 16 == 0 ? 0 : 1);
    int num7 = height / 16 + (height % 16 == 0 ? 0 : 1);
    if (num1 != 0)
    {
        for (int index3 = 0; index3 < num7; ++index3)
        {
            if (Main.tile[index1, index2 + index3 * num5] != null)
                points.Add(new Tuple<int, int>(index1, index2 + index3 * num5));
        }
    }
    if (num2 != 0)
    {
        for (int index3 = 0; index3 < num6; ++index3)
        {
            if (Main.tile[(int)((double)position.X / 16.0) + index3, index2] != null)
                points.Add(new Tuple<int, int>((int)((double)position.X / 16.0) + index3, index2));
        }
    }
    int x = (int)(((double)providedInfo.mouse.X + (double)((width / 2 - 1) * num1)) / 16.0);
    int y = (int)(((double)providedInfo.mouse.Y + 0.1 - (double)(height / 2 + 1)) / 16.0);
    if (num5 == -1)
        y = (int)(((double)providedInfo.mouse.Y + (double)(height / 2) - 1.0) / 16.0);
    if ((double)gravDir == -1.0 && num2 == 0)
        ++y;
    if (y < 10)
        y = 10;
    if (y > Main.maxTilesY - 10)
        y = Main.maxTilesY - 10;
    int num8 = width / 16 + (width % 16 == 0 ? 0 : 1);
    int num9 = height / 16 + (height % 16 == 0 ? 0 : 1);
    // WorldGen.InWorld(x, y, Main.Map.BlackEdgeWidth)
    if (true)
    {
        if (num1 != 0)
        {
            for (int index3 = 0; index3 < num9; ++index3)
            {
                if (Main.tile[x, y + index3 * num5] != null)
                    endpoints.Add(new Tuple<int, int>(x, y + index3 * num5));
            }
        }
        if (num2 != 0)
        {
            for (int index3 = 0; index3 < num8; ++index3)
            {
                if (Main.tile[(int)(((double)providedInfo.mouse.X - (double)(width / 2)) / 16.0) + index3, y] != null)
                    endpoints.Add(new Tuple<int, int>((int)(((double)providedInfo.mouse.X - (double)(width / 2)) / 16.0) + index3, y));
            }
        }
    }
    targets.Clear();
    while (points.Count > 0 && endpoints.Count > 0)
    {
        Tuple<int, int> point = points[0];
        Tuple<int, int> endpoint = endpoints[0];
        Tuple<int, int> col;
        if (!TupleHitLine(point.Item1, point.Item2, endpoint.Item1, endpoint.Item2, num1 * (int)gravDir, -num2 * (int)gravDir, grappleTargets, out col))
        {
            points.Remove(point);
            endpoints.Remove(endpoint);
        }
        else
        {
            if (col.Item1 != endpoint.Item1 || col.Item2 != endpoint.Item2)
                targets.Add(col);
            Tile tile = Main.tile[col.Item1, col.Item2];
            if (tile.active && (Main.tileSolid[(int)tile.type] && !Main.tileSolidTop[(int)tile.type]) && !grappleTargets.Contains(col))
                targets.Add(col);
            points.Remove(point);
            endpoints.Remove(endpoint);
        }
    }
    toRemove.Clear();
    for (int index3 = 0; index3 < targets.Count; ++index3)
    {
        // WorldGen.CanKillTile(targets[index3].Item1, targets[index3].Item2)
        if (!true)
            toRemove.Add(targets[index3]);
    }
    for (int index3 = 0; index3 < toRemove.Count; ++index3)
        targets.Remove(toRemove[index3]);
    toRemove.Clear();
    if (targets.Count > 0)
    {
        float num10 = -1f;
        Tuple<int, int> target = targets[0];
        Vector2 vector2_2 = center;
        // Netcode, ignore for now

        /* if (Main.netMode == 1)
        {
            int num11 = 0;
            int num12 = 0;
            int num13 = 0;
            for (int index3 = 0; index3 < whoAmI; ++index3)
            {
                Player player1 = Main.player[index3];
                if (player1.active && !player1.dead && (player1.HeldItem.pick > 0 && player1.itemAnimation > 0))
                {
                    if ((double)player.Distance(player1.Center) <= 8.0)
                        ++num11;
                    if ((double)player.Distance(player1.Center) <= 80.0 && (double)Math.Abs(player1.Center.Y - center.Y) <= 12.0)
                        ++num12;
                }
            }
            for (int index3 = whoAmI + 1; index3 < (int)byte.MaxValue; ++index3)
            {
                Player player1 = Main.player[index3];
                if (player1.active && !player1.dead && (player1.HeldItem.pick > 0 && player1.itemAnimation > 0) && (double)player.Distance(player1.Center) <= 8.0)
                    ++num13;
            }
            if (num11 > 0)
            {
                if (num11 % 2 == 1)
                    vector2_2.X += 12f;
                else
                    vector2_2.X -= 12f;
                if (num12 % 2 == 1)
                    vector2_2.Y -= 12f;
            }
            if (num13 > 0 && num11 == 0)
            {
                if (num13 % 2 == 1)
                    vector2_2.X -= 12f;
                else
                    vector2_2.X += 12f;
            }
        } */
        for (int index3 = 0; index3 < targets.Count; ++index3)
        {
            float num11 = Vector2.Distance(new Vector2((float)targets[index3].Item1, (float)targets[index3].Item2) * 16f + Vector2.One * 8f, vector2_2);
            if ((double)num10 == -1.0 || (double)num11 < (double)num10)
            {
                num10 = num11;
                target = targets[index3];
            }
        }
        if (InTileBounds(target.Item1, target.Item2, providedInfo.reachableStartX, providedInfo.reachableStartY, providedInfo.reachableEndX, providedInfo.reachableEndY))
        {
            focusedX = target.Item1;
            focusedY = target.Item2;
        }
    }
    points.Clear();
    endpoints.Clear();
    targets.Clear();
}
#endregion

#region Axe
private static void Axe(
  SmartCursorUsageInfo providedInfo,
  ref int fX,
  ref int fY)
{
    int reachableStartX = providedInfo.reachableStartX;
    int reachableStartY = providedInfo.reachableStartY;
    int reachableEndX = providedInfo.reachableEndX;
    int reachableEndY = providedInfo.reachableEndY;
    int screenTargetX = providedInfo.screenTargetX;
    int screenTargetY = providedInfo.screenTargetY;
    if (providedInfo.item.axe <= 0 || fX != -1 || fY != -1)
        return;
    float num1 = -1f;
    for (int index1 = reachableStartX; index1 <= reachableEndX; ++index1)
    {
        for (int index2 = reachableStartY; index2 <= reachableEndY; ++index2)
        {
            if (Main.tile[index1, index2].active)
            {
                Tile tile = Main.tile[index1, index2];
                // && !TileID.Sets.IgnoreSmartCursorPriorityAxe[(int)tile.type]
                if (Main.tileAxe[(int)tile.type])
                {
                    int x = index1;
                    int y = index2;
                    int type = (int)tile.type;
                    // TileID.Sets.IsATreeTrunk[type], 5 is the tree id
                    if (type == 5)
                    {
                        // Collision.InTileBounds
                        if (InTileBounds(x + 1, y, reachableStartX, reachableStartY, reachableEndX, reachableEndY))
                        {
                            if (Main.tile[x, y].frameY >= (short)198 && Main.tile[x, y].frameX == (short)44)
                                ++x;
                            if (Main.tile[x, y].frameX == (short)66 && Main.tile[x, y].frameY <= (short)44)
                                ++x;
                            if (Main.tile[x, y].frameX == (short)44 && Main.tile[x, y].frameY >= (short)132 && Main.tile[x, y].frameY <= (short)176)
                                ++x;
                        }
                        if (InTileBounds(x - 1, y, reachableStartX, reachableStartY, reachableEndX, reachableEndY))
                        {
                            if (Main.tile[x, y].frameY >= (short)198 && Main.tile[x, y].frameX == (short)66)
                                --x;
                            if (Main.tile[x, y].frameX == (short)88 && Main.tile[x, y].frameY >= (short)66 && Main.tile[x, y].frameY <= (short)110)
                                --x;
                            if (Main.tile[x, y].frameX == (short)22 && Main.tile[x, y].frameY >= (short)132 && Main.tile[x, y].frameY <= (short)176)
                                --x;
                        }
                        while (Main.tile[x, y].active && (int)Main.tile[x, y].type == type && ((int)Main.tile[x, y + 1].type == type && InTileBounds(x, y + 1, reachableStartX, reachableStartY, reachableEndX, reachableEndY)))
                            ++y;
                    }
                    if (tile.type == (ushort)80)
                    {
                        if (InTileBounds(x + 1, y, reachableStartX, reachableStartY, reachableEndX, reachableEndY))
                        {
                            if (Main.tile[x, y].frameX == (short)54)
                                ++x;
                            if (Main.tile[x, y].frameX == (short)108 && Main.tile[x, y].frameY == (short)36)
                                ++x;
                        }
                        if (InTileBounds(x - 1, y, reachableStartX, reachableStartY, reachableEndX, reachableEndY))
                        {
                            if (Main.tile[x, y].frameX == (short)36)
                                --x;
                            if (Main.tile[x, y].frameX == (short)108 && Main.tile[x, y].frameY == (short)18)
                                --x;
                        }
                        while (Main.tile[x, y].active && Main.tile[x, y].type == (ushort)80 && (Main.tile[x, y + 1].type == (ushort)80 && InTileBounds(x, y + 1, reachableStartX, reachableStartY, reachableEndX, reachableEndY)))
                            ++y;
                    }
                    if (tile.type == (ushort)323 || tile.type == (ushort)72)
                    {
                        while (Main.tile[x, y].active && (Main.tile[x, y].type == (ushort)323 && Main.tile[x, y + 1].type == (ushort)323 || Main.tile[x, y].type == (ushort)72 && Main.tile[x, y + 1].type == (ushort)72) && InTileBounds(x, y + 1, reachableStartX, reachableStartY, reachableEndX, reachableEndY))
                            ++y;
                    }
                    float num2 = Vector2.Distance(new Vector2((float)x, (float)y) * 16f + Vector2.One * 8f, providedInfo.mouse);
                    if ((double)num1 == -1.0 || (double)num2 < (double)num1)
                    {
                        num1 = num2;
                        fX = x;
                        fY = y;
                    }
                }
            }
        }
    }
}
#endregion

#region Hammers
private static void Hammers(
  SmartCursorUsageInfo providedInfo,
  ref int focusedX,
  ref int focusedY)
{
    int width = providedInfo.player.width;
    int height = providedInfo.player.height;
    if (providedInfo.item.hammer > 0 && focusedX == -1 && focusedY == -1)
    {
        Vector2 vector2 = providedInfo.mouse - providedInfo.Center;
        int num1 = Math.Sign(vector2.X);
        int num2 = Math.Sign(vector2.Y);
        if ((double)Math.Abs(vector2.X) > (double)Math.Abs(vector2.Y) * 3.0)
        {
            num2 = 0;
            providedInfo.mouse.Y = providedInfo.Center.Y;
        }
        if ((double)Math.Abs(vector2.Y) > (double)Math.Abs(vector2.X) * 3.0)
        {
            num1 = 0;
            providedInfo.mouse.X = providedInfo.Center.X;
        }
        int num3 = (int)providedInfo.Center.X / 16;
        int num4 = (int)providedInfo.Center.Y / 16;
        points.Clear();
        endpoints.Clear();
        int num5 = 1;
        if (num2 == -1 && num1 != 0)
            num5 = -1;
        int index1 = (int)(((double)providedInfo.position.X + (double)(width / 2) + (double)((width / 2 - 1) * num1)) / 16.0);
        int index2 = (int)(((double)providedInfo.position.Y + 0.1) / 16.0);
        if (num5 == -1)
            index2 = (int)(((double)providedInfo.position.Y + (double)height - 1.0) / 16.0);
        int num6 = width / 16 + (width % 16 == 0 ? 0 : 1);
        int num7 = height / 16 + (height % 16 == 0 ? 0 : 1);
        if (num1 != 0)
        {
            for (int index3 = 0; index3 < num7; ++index3)
            {
                if (Main.tile[index1, index2 + index3 * num5] != null)
                    points.Add(new Tuple<int, int>(index1, index2 + index3 * num5));
            }
        }
        if (num2 != 0)
        {
            for (int index3 = 0; index3 < num6; ++index3)
            {
                if (Main.tile[(int)((double)providedInfo.position.X / 16.0) + index3, index2] != null)
                    points.Add(new Tuple<int, int>((int)((double)providedInfo.position.X / 16.0) + index3, index2));
            }
        }
        int index4 = (int)(((double)providedInfo.mouse.X + (double)((width / 2 - 1) * num1)) / 16.0);
        int index5 = (int)(((double)providedInfo.mouse.Y + 0.1 - (double)(height / 2 + 1)) / 16.0);
        if (num5 == -1)
            index5 = (int)(((double)providedInfo.mouse.Y + (double)(height / 2) - 1.0) / 16.0);
        if ((double)providedInfo.player.gravDir == -1.0 && num2 == 0)
            ++index5;
        if (index5 < 10)
            index5 = 10;
        if (index5 > Main.maxTilesY - 10)
            index5 = Main.maxTilesY - 10;
        int num8 = width / 16 + (width % 16 == 0 ? 0 : 1);
        int num9 = height / 16 + (height % 16 == 0 ? 0 : 1);
        if (num1 != 0)
        {
            for (int index3 = 0; index3 < num9; ++index3)
            {
                if (Main.tile[index4, index5 + index3 * num5] != null)
                    endpoints.Add(new Tuple<int, int>(index4, index5 + index3 * num5));
            }
        }
        if (num2 != 0)
        {
            for (int index3 = 0; index3 < num8; ++index3)
            {
                if (Main.tile[(int)(((double)providedInfo.mouse.X - (double)(width / 2)) / 16.0) + index3, index5] != null)
                    endpoints.Add(new Tuple<int, int>((int)(((double)providedInfo.mouse.X - (double)(width / 2)) / 16.0) + index3, index5));
            }
        }

        targets.Clear();
        while (points.Count > 0)
        {
            Tuple<int, int> point = points[0];
            Tuple<int, int> endpoint = endpoints[0];
            Tuple<int, int> tuple = TupleHitLineWall(point.Item1, point.Item2, endpoint.Item1, endpoint.Item2);
            if (tuple.Item1 == -1 || tuple.Item2 == -1)
            {
                points.Remove(point);
                endpoints.Remove(endpoint);
            }
            else
            {
                if (tuple.Item1 != endpoint.Item1 || tuple.Item2 != endpoint.Item2)
                    targets.Add(tuple);
                Tile tile = Main.tile[tuple.Item1, tuple.Item2];
                if (HitWallSubstep(tuple.Item1, tuple.Item2))
                    targets.Add(tuple);
                points.Remove(point);
                endpoints.Remove(endpoint);
            }
        }
        if (targets.Count > 0)
        {
            float num10 = -1f;
            Tuple<int, int> tuple = (Tuple<int, int>)null;
            for (int index3 = 0; index3 < targets.Count; ++index3)
            {
                if (!Main.tile[targets[index3].Item1, targets[index3].Item2].active || Main.tile[targets[index3].Item1, targets[index3].Item2].type != (ushort)26)
                {
                    float num11 = Vector2.Distance(new Vector2((float)targets[index3].Item1, (float)targets[index3].Item2) * 16f + Vector2.One * 8f, providedInfo.Center);
                    if ((double)num10 == -1.0 || (double)num11 < (double)num10)
                    {
                        num10 = num11;
                        tuple = targets[index3];
                    }
                }
            }
            if (tuple != null && InTileBounds(tuple.Item1, tuple.Item2, providedInfo.reachableStartX, providedInfo.reachableStartY, providedInfo.reachableEndX, providedInfo.reachableEndY))
            {
                // providedInfo.player.poundRelease = false;
                focusedX = tuple.Item1;
                focusedY = tuple.Item2;
            }
        }
        targets.Clear();
        points.Clear();
        endpoints.Clear();
    }
    if (providedInfo.item.hammer <= 0 || focusedX != -1 || focusedY != -1)
        return;
    targets.Clear();
    for (int reachableStartX = providedInfo.reachableStartX; reachableStartX <= providedInfo.reachableEndX; ++reachableStartX)
    {
        for (int reachableStartY = providedInfo.reachableStartY; reachableStartY <= providedInfo.reachableEndY; ++reachableStartY)
        {
            if (Main.tile[reachableStartX, reachableStartY].wall > (ushort)0 && HitWallSubstep(reachableStartX, reachableStartY))
                targets.Add(new Tuple<int, int>(reachableStartX, reachableStartY));
        }
    }
    if (targets.Count > 0)
    {
        float num1 = -1f;
        Tuple<int, int> tuple = (Tuple<int, int>)null;
        for (int index = 0; index < targets.Count; ++index)
        {
            if (!Main.tile[targets[index].Item1, targets[index].Item2].active || Main.tile[targets[index].Item1, targets[index].Item2].type != (ushort)26)
            {
                float num2 = Vector2.Distance(new Vector2((float)targets[index].Item1, (float)targets[index].Item2) * 16f + Vector2.One * 8f, providedInfo.mouse);
                if ((double)num1 == -1.0 || (double)num2 < (double)num1)
                {
                    num1 = num2;
                    tuple = targets[index];
                }
            }
        }
        if (tuple != null && InTileBounds(tuple.Item1, tuple.Item2, providedInfo.reachableStartX, providedInfo.reachableStartY, providedInfo.reachableEndX, providedInfo.reachableEndY))
        {
            // providedInfo.player.poundRelease = false;
            focusedX = tuple.Item1;
            focusedY = tuple.Item2;
        }
    }
    targets.Clear();
}
#endregion

#region Walls
private static void Walls(
      SmartCursorUsageInfo providedInfo,
      ref int focusedX,
      ref int focusedY)
{
    int width = providedInfo.player.width;
    int height = providedInfo.player.height;
    if (providedInfo.item.createWall <= 0 || focusedX != -1 || focusedY != -1)
        return;
    targets.Clear();
    for (int reachableStartX = providedInfo.reachableStartX; reachableStartX <= providedInfo.reachableEndX; ++reachableStartX)
    {
        for (int reachableStartY = providedInfo.reachableStartY; reachableStartY <= providedInfo.reachableEndY; ++reachableStartY)
        {
            Tile tile = Main.tile[reachableStartX, reachableStartY];
            // tile.wall == (ushort) 0 && (!tile.active() || !Main.tileSolid[(int) tile.type] || Main.tileSolidTop[(int) tile.type]) && Collision.CanHitWithCheck(providedInfo.position, width, height, new Vector2((float) reachableStartX, (float) reachableStartY) * 16f, 16, 16, new Utils.TileActionAttempt(DelegateMethods.NotDoorStand))
            if (tile.wall == (ushort)0 && (!tile.active || !Main.tileSolid[(int)tile.type] || Main.tileSolidTop[(int)tile.type]) && CanHitWithCheck(providedInfo.position, width, height, new Vector2((float)reachableStartX, (float)reachableStartY) * 16f, 16, 16))
            {
                // active()
                bool flag = false;
                if (Main.tile[reachableStartX - 1, reachableStartY].active || Main.tile[reachableStartX - 1, reachableStartY].wall > (ushort)0)
                    flag = true;
                if (Main.tile[reachableStartX + 1, reachableStartY].active || Main.tile[reachableStartX + 1, reachableStartY].wall > (ushort)0)
                    flag = true;
                if (Main.tile[reachableStartX, reachableStartY - 1].active || Main.tile[reachableStartX, reachableStartY - 1].wall > (ushort)0)
                    flag = true;
                if (Main.tile[reachableStartX, reachableStartY + 1].active || Main.tile[reachableStartX, reachableStartY + 1].wall > (ushort)0)
                    flag = true;
                if (IsOpenDoorAnchorFrame(reachableStartX, reachableStartY))
                    flag = false;
                if (flag)
                    targets.Add(new Tuple<int, int>(reachableStartX, reachableStartY));
            }
        }
    }
    if (targets.Count > 0)
    {
        float num1 = -1f;
        Tuple<int, int> target = targets[0];
        for (int index = 0; index < targets.Count; ++index)
        {
            // Vector2.Distance(new Vector2((float)SmartCursorHelper._targets[index].Item1, (float)SmartCursorHelper._targets[index].Item2) * 16f + Vector2.One * 8f, providedInfo.mouse);
            float num2 = Vector2.Distance(new Vector2((float)targets[index].Item1, (float)targets[index].Item2) * 16f + Vector2.One * 8f, providedInfo.mouse);
            if ((double)num1 == -1.0 || (double)num2 < (double)num1)
            {
                num1 = num2;
                target = targets[index];
            }
        }
        if (InTileBounds(target.Item1, target.Item2, providedInfo.reachableStartX, providedInfo.reachableStartY, providedInfo.reachableEndX, providedInfo.reachableEndY))
        {
            focusedX = target.Item1;
            focusedY = target.Item2;
        }
    }
    targets.Clear();

}
#endregion

#region Torches
private static void Torch(
      SmartCursorUsageInfo providedInfo,
      ref int fX,
      ref int fY)
{
    int reachableStartX = providedInfo.reachableStartX;
    int reachableStartY = providedInfo.reachableStartY;
    int reachableEndX = providedInfo.reachableEndX;
    int reachableEndY = providedInfo.reachableEndY;
    int screenTargetX = providedInfo.screenTargetX;
    int screenTargetY = providedInfo.screenTargetY;
    if (providedInfo.item.createTile != 4 || fX != -1 || fY != -1)
        return;
    targets.Clear();
    for (int index1 = reachableStartX; index1 <= reachableEndX; ++index1)
    {
        for (int index2 = reachableStartY; index2 <= reachableEndY; ++index2)
        {
            Tile tile = Main.tile[index1, index2]; // tile1
            Tile tileLeft= Main.tile[index1 - 1, index2]; // tile2
            Tile tileRight = Main.tile[index1 + 1, index2]; // tile3
            Tile tileUp = Main.tile[index1, index2 + 1]; // tile4
            // TileID.Sets.BreakableWhenPlacing[(int)tile1.type] removed
            if (!tile.active || Main.tileCut[(int)tile.type] && tile.type != (ushort)82 && tile.type != (ushort)83)
            {
                bool cantPlace = false;
                for (int index3 = index1 - 8; index3 <= index1 + 8; ++index3)
                {
                    for (int index4 = index2 - 8; index4 <= index2 + 8; ++index4)
                    {
                        if (Main.tile[index3, index4] != null && Main.tile[index3, index4].type == (ushort)4)
                        {
                            cantPlace = true;
                            break;
                        }
                    }
                    if (cantPlace)
                        break;
                }
                // WorldGen.IsTreeType((int)Main.tile[index1 + 1, index2 + 1].type)) => (int)tile.type == 5 

                if (!cantPlace
                    && (tile.liquid <= (byte)0)
                    && (tileLeft.active
                        //&& (tileLeft.slope() == (byte)0 || (int)tileLeft.slope() % 2 != 1) 
                        && (Main.tileSolid[(int)tileLeft.type]
                            && !Main.tileNoAttach[(int)tileLeft.type]
                            && (!Main.tileSolidTop[(int)tileLeft.type] && !isNotReallySolid((int)tileLeft.type))
                            || isBeamType((int)tileLeft.type)
                            || isTreeType((int)tileLeft.type)
                            && isTreeType((int)Main.tile[index1 - 1, index2 - 1].type)
                            && isTreeType((int)Main.tile[index1 - 1, index2 + 1].type))
                        || (tileRight.active
                            //&& (tileRight.slope() == (byte)0 || (int)tileRight.slope() % 2 != 0)
                            && (Main.tileSolid[(int)tileRight.type]
                                && !Main.tileNoAttach[(int)tileRight.type]
                                && (!Main.tileSolidTop[(int)tileRight.type] && !isNotReallySolid((int)tileRight.type))
                                || isBeamType((int)tileRight.type)
                                || isTreeType((int)tileRight.type)
                                && isTreeType((int)Main.tile[index1 + 1, index2 - 1].type)
                                && isTreeType((int)Main.tile[index1 + 1, index2 + 1].type))
                            || tileUp.active
                               && Main.tileSolid[(int)tileUp.type]
                               && !Main.tileNoAttach[(int)tileUp.type]
                               && (!Main.tileSolidTop[(int)tileUp.type] || isPlatform((int)tileUp.type))
                               && (!isNotReallySolid((int)tileUp.type))))
                    && tile.type != (ushort)4)
                    targets.Add(new Tuple<int, int>(index1, index2));
            }
        }
    }
    if (targets.Count > 0)
    {
        float num1 = -1f;
        Tuple<int, int> target = targets[0];
        for (int index = 0; index < targets.Count; ++index)
        {
            float num2 = Vector2.Distance(new Vector2((float)targets[index].Item1, (float)targets[index].Item2) * 16f + Vector2.One * 8f, providedInfo.mouse);
            if ((double)num1 == -1.0 || (double)num2 < (double)num1)
            {
                num1 = num2;
                target = targets[index];
            }
        }
        if (InTileBounds(target.Item1, target.Item2, reachableStartX, reachableStartY, reachableEndX, reachableEndY))
        {
            fX = target.Item1;
            fY = target.Item2;
        }
    }
    targets.Clear();
}
#endregion

#region EmptyBuckets
private static void EmptyBuckets(
   SmartCursorUsageInfo providedInfo,
   ref int focusedX,
   ref int focusedY)
{
    if (providedInfo.item.type != 205 || focusedX != -1 || focusedY != -1)
        return;
    targets.Clear();
    for (int reachableStartX = providedInfo.reachableStartX; reachableStartX <= providedInfo.reachableEndX; ++reachableStartX)
    {
        for (int reachableStartY = providedInfo.reachableStartY; reachableStartY <= providedInfo.reachableEndY; ++reachableStartY)
        {
            Tile tile = Main.tile[reachableStartX, reachableStartY];
            if (tile.liquid > (byte)0)
            {
                // 
                // int num1 = (int)tile.liquidType();
                int num2 = 0;
                for (int index1 = reachableStartX - 1; index1 <= reachableStartX + 1; ++index1)
                {
                    for (int index2 = reachableStartY - 1; index2 <= reachableStartY + 1; ++index2)
                    {
                        // if ((int)Main.tile[index1, index2].liquidType() == num1)
                        num2 += (int)Main.tile[index1, index2].liquid;
                    }
                }
                if (num2 > 100)
                    targets.Add(new Tuple<int, int>(reachableStartX, reachableStartY));
            }
        }
    }
    if (targets.Count > 0)
    {
        float num1 = -1f;
        Tuple<int, int> target = targets[0];
        for (int index = 0; index < targets.Count; ++index)
        {
            float num2 = Vector2.Distance(new Vector2((float)targets[index].Item1, (float)targets[index].Item2) * 16f + Vector2.One * 8f, providedInfo.mouse);
            if ((double)num1 == -1.0 || (double)num2 < (double)num1)
            {
                num1 = num2;
                target = targets[index];
            }
        }
        if (InTileBounds(target.Item1, target.Item2, providedInfo.reachableStartX, providedInfo.reachableStartY, providedInfo.reachableEndX, providedInfo.reachableEndY))
        {
            focusedX = target.Item1;
            focusedY = target.Item2;
        }
    }
    targets.Clear();
}
#endregion

#region HelperMethods
public static bool isTreeType(int type)
{
    return type == 5;
}

public static bool isBeamType(int type)
{
    return type == 124;
}

public static bool isNotReallySolid(int type)
{
    return type == 10 || type == 11;
}

public static bool isPlatform(int type)
{
    return type == 19;
}


public static bool IsOpenDoorAnchorFrame(int x, int y)
{
    Tile tile = Main.tile[x, y];
    // !active || tile.type != (ushort)11
    if (!tile.active || tile.type != (ushort)11)
        return false;
    int num = (int)tile.frameX % 72;
    return num < 18 || num >= 54;
}

#region CanHitWithCheck
// public static bool CanHitWithCheck(Vector2 Position1, int Width1, int Height1, Vector2 Position 2, int Width2, int Height2, Utils.TileActionAttempt check)
public static bool CanHitWithCheck(
  Vector2 Position1,
  int Width1,
  int Height1,
  Vector2 Position2,
  int Width2,
  int Height2)
{
    int x = (int)(((double)Position1.X + (double)(Width1 / 2)) / 16.0);
    int y = (int)(((double)Position1.Y + (double)(Height1 / 2)) / 16.0);
    int num1 = (int)(((double)Position2.X + (double)(Width2 / 2)) / 16.0);
    int num2 = (int)(((double)Position2.Y + (double)(Height2 / 2)) / 16.0);
    if (x <= 1)
        x = 1;
    if (x >= Main.maxTilesX)
        x = Main.maxTilesX - 1;
    if (num1 <= 1)
        num1 = 1;
    if (num1 >= Main.maxTilesX)
        num1 = Main.maxTilesX - 1;
    if (y <= 1)
        y = 1;
    if (y >= Main.maxTilesY)
        y = Main.maxTilesY - 1;
    if (num2 <= 1)
        num2 = 1;
    if (num2 >= Main.maxTilesY)
        num2 = Main.maxTilesY - 1;
    try
    {
        do
        {
            int num3 = Math.Abs(x - num1);
            int num4 = Math.Abs(y - num2);
            if (x == num1 && y == num2)
                return true;
            if (num3 > num4)
            {
                if (x < num1)
                    ++x;
                else
                    --x;
                // Main.tile[x, y - 1] == null || Main.tile[x, y + 1] == null || !Main.tile[x, y - 1].inActive() && Main.tile[x, y - 1].active() && (Main.tileSolid[(int)Main.tile[x, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y - 1].type]) && (Main.tile[x, y - 1].slope() == (byte)0 && !Main.tile[x, y - 1].halfBrick() && (!Main.tile[x, y + 1].inActive() && Main.tile[x, y + 1].active())) && (Main.tileSolid[(int)Main.tile[x, y + 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y + 1].type] && (Main.tile[x, y + 1].slope() == (byte)0 && !Main.tile[x, y + 1].halfBrick()))
                if (Main.tile[x, y - 1] == null || Main.tile[x, y + 1] == null && Main.tile[x, y - 1].active && (Main.tileSolid[(int)Main.tile[x, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y - 1].type]) && (Main.tile[x, y + 1].active) && (Main.tileSolid[(int)Main.tile[x, y + 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y + 1].type]))
                    return false;
            }
            else
            {
                if (y < num2)
                    ++y;
                else
                    --y;
                // Main.tile[x - 1, y] == null || Main.tile[x + 1, y] == null || !Main.tile[x - 1, y].inActive() && Main.tile[x - 1, y].active() && (Main.tileSolid[(int)Main.tile[x - 1, y].type] && !Main.tileSolidTop[(int)Main.tile[x - 1, y].type]) && (Main.tile[x - 1, y].slope() == (byte)0 && !Main.tile[x - 1, y].halfBrick() && (!Main.tile[x + 1, y].inActive() && Main.tile[x + 1, y].active())) && (Main.tileSolid[(int)Main.tile[x + 1, y].type] && !Main.tileSolidTop[(int)Main.tile[x + 1, y].type] && (Main.tile[x + 1, y].slope() == (byte)0 && !Main.tile[x + 1, y].halfBrick()))
                if (Main.tile[x - 1, y] == null || Main.tile[x + 1, y] == null && Main.tile[x - 1, y].active && (Main.tileSolid[(int)Main.tile[x - 1, y].type] && !Main.tileSolidTop[(int)Main.tile[x - 1, y].type]) && (Main.tile[x + 1, y].active) && (Main.tileSolid[(int)Main.tile[x + 1, y].type] && !Main.tileSolidTop[(int)Main.tile[x + 1, y].type]))
                    return false;
            }
        }
        // Main.tile[x, y] != null && (Main.tile[x, y].inActive() || !Main.tile[x, y].active() || (!Main.tileSolid[(int)Main.tile[x, y].type] || Main.tileSolidTop[(int)Main.tile[x, y].type])) && check(x, y)
        while (Main.tile[x, y] != null || !Main.tile[x, y].active || (!Main.tileSolid[(int)Main.tile[x, y].type] || Main.tileSolidTop[(int)Main.tile[x, y].type]));
        return false;
    }
    catch
    {
        return false;
    }
}
#endregion

public static bool NotDoorStand(int x, int y)
{
    // Main.tile[x, y] == null || !Main.tile[x, y].active() || Main.tile[x, y].type != (ushort)11
    if (Main.tile[x, y] == null || !Main.tile[x, y].active || Main.tile[x, y].type != (ushort)11)
        return true;
    return Main.tile[x, y].frameX >= (short)18 && Main.tile[x, y].frameX < (short)54;
}

public static Tuple<int, int> TupleHitLineWall(int x1, int y1, int x2, int y2)
{
    int x = x1;
    int y = y1;
    int num1 = x2;
    int num2 = y2;
    if (x <= 1)
        x = 1;
    if (x >= Main.maxTilesX)
        x = Main.maxTilesX - 1;
    if (num1 <= 1)
        num1 = 1;
    if (num1 >= Main.maxTilesX)
        num1 = Main.maxTilesX - 1;
    if (y <= 1)
        y = 1;
    if (y >= Main.maxTilesY)
        y = Main.maxTilesY - 1;
    if (num2 <= 1)
        num2 = 1;
    if (num2 >= Main.maxTilesY)
        num2 = Main.maxTilesY - 1;
    float num3 = (float)Math.Abs(x - num1);
    float num4 = (float)Math.Abs(y - num2);
    if ((double)num3 == 0.0 && (double)num4 == 0.0)
        return new Tuple<int, int>(x, y);
    float num5 = 1f;
    float num6 = 1f;
    if ((double)num3 == 0.0 || (double)num4 == 0.0)
    {
        if ((double)num3 == 0.0)
            num5 = 0.0f;
        if ((double)num4 == 0.0)
            num6 = 0.0f;
    }
    else if ((double)num3 > (double)num4)
        num5 = num3 / num4;
    else
        num6 = num4 / num3;
    float num7 = 0.0f;
    float num8 = 0.0f;
    int num9 = 1;
    if (y < num2)
        num9 = 2;
    int num10 = (int)num3;
    int num11 = (int)num4;
    int num12 = Math.Sign(num1 - x);
    int num13 = Math.Sign(num2 - y);
    bool flag1 = false;
    bool flag2 = false;
    try
    {
        do
        {
            switch (num9)
            {
                case 1:
                    float num14 = num8 + num6;
                    int num15 = (int)num14;
                    num8 = num14 % 1f;
                    for (int index = 0; index < num15; ++index)
                    {
                        Tile tile = Main.tile[x, y];
                        if (HitWallSubstep(x, y))
                            return new Tuple<int, int>(x, y);
                        if (num10 == 0 && num11 == 0)
                        {
                            flag1 = true;
                            break;
                        }
                        y += num13;
                        --num11;
                        if (num10 == 0 && num11 == 0 && num15 == 1)
                            flag2 = true;
                    }
                    if (num10 != 0)
                    {
                        num9 = 2;
                        break;
                    }
                    break;
                case 2:
                    float num16 = num7 + num5;
                    int num17 = (int)num16;
                    num7 = num16 % 1f;
                    for (int index = 0; index < num17; ++index)
                    {
                        Tile tile = Main.tile[x, y];
                        if (HitWallSubstep(x, y))
                            return new Tuple<int, int>(x, y);
                        if (num10 == 0 && num11 == 0)
                        {
                            flag1 = true;
                            break;
                        }
                        x += num12;
                        --num10;
                        if (num10 == 0 && num11 == 0 && num17 == 1)
                            flag2 = true;
                    }
                    if (num11 != 0)
                    {
                        num9 = 1;
                        break;
                    }
                    break;
            }
            if (Main.tile[x, y] == null)
                return new Tuple<int, int>(-1, -1);
            Tile tile1 = Main.tile[x, y];
            if (HitWallSubstep(x, y))
                return new Tuple<int, int>(x, y);
        }
        while (!(flag1 | flag2));
        return new Tuple<int, int>(x, y);
    }
    catch
    {
        return new Tuple<int, int>(-1, -1);
    }
}

public static bool HitWallSubstep(int x, int y)
{
    if (Main.tile[x, y].wall == (ushort)0)
        return false;
    bool flag1 = false;
    if (Main.wallHouse[(int)Main.tile[x, y].wall])
        flag1 = true;
    if (!flag1)
    {
        for (int index1 = -1; index1 < 2; ++index1)
        {
            for (int index2 = -1; index2 < 2; ++index2)
            {
                if ((index1 != 0 || index2 != 0) && Main.tile[x + index1, y + index2].wall == (ushort)0)
                    flag1 = true;
            }
        }
    }
    if (Main.tile[x, y].active & flag1)
    {
        bool flag2 = true;
        for (int index1 = -1; index1 < 2; ++index1)
        {
            for (int index2 = -1; index2 < 2; ++index2)
            {
                if (index1 != 0 || index2 != 0)
                {
                    Tile tile = Main.tile[x + index1, y + index2];
                    if (!tile.active || !Main.tileSolid[(int)tile.type] || Main.tileSolidTop[(int)tile.type])
                        flag2 = false;
                }
            }
        }
        if (flag2)
            flag1 = false;
    }
    return flag1;
}

#region TupleHitLine
public static bool TupleHitLine(
      int x1,
      int y1,
      int x2,
      int y2,
      int ignoreX,
      int ignoreY,
      List<Tuple<int, int>> ignoreTargets,
      out Tuple<int, int> col)
{
    int num1 = x1;
    int num2 = y1;
    int num3 = x2;
    int num4 = y2;
    int index1 = Clamp<int>(num1, 1, Main.maxTilesX - 1);
    int num5 = Clamp<int>(num3, 1, Main.maxTilesX - 1);
    int index2 = Clamp<int>(num2, 1, Main.maxTilesY - 1);
    int num6 = Clamp<int>(num4, 1, Main.maxTilesY - 1);
    float num7 = (float)Math.Abs(index1 - num5);
    float num8 = (float)Math.Abs(index2 - num6);
    if ((double)num7 == 0.0 && (double)num8 == 0.0)
    {
        col = new Tuple<int, int>(index1, index2);
        return true;
    }
    float num9 = 1f;
    float num10 = 1f;
    if ((double)num7 == 0.0 || (double)num8 == 0.0)
    {
        if ((double)num7 == 0.0)
            num9 = 0.0f;
        if ((double)num8 == 0.0)
            num10 = 0.0f;
    }
    else if ((double)num7 > (double)num8)
        num9 = num7 / num8;
    else
        num10 = num8 / num7;
    float num11 = 0.0f;
    float num12 = 0.0f;
    int num13 = 1;
    if (index2 < num6)
        num13 = 2;
    int num14 = (int)num7;
    int num15 = (int)num8;
    int num16 = Math.Sign(num5 - index1);
    int num17 = Math.Sign(num6 - index2);
    bool flag1 = false;
    bool flag2 = false;
    try
    {
        do
        {
            switch (num13)
            {
                case 1:
                    float num18 = num12 + num10;
                    int num19 = (int)num18;
                    num12 = num18 % 1f;
                    for (int index3 = 0; index3 < num19; ++index3)
                    {
                        if (Main.tile[index1 - 1, index2] == null)
                        {
                            col = new Tuple<int, int>(index1 - 1, index2);
                            return false;
                        }
                        if (Main.tile[index1 + 1, index2] == null)
                        {
                            col = new Tuple<int, int>(index1 + 1, index2);
                            return false;
                        }
                        Tile tile1 = Main.tile[index1 - 1, index2];
                        Tile tile2 = Main.tile[index1 + 1, index2];
                        Tile tile3 = Main.tile[index1, index2];
                        if (!ignoreTargets.Contains(new Tuple<int, int>(index1, index2)) && !ignoreTargets.Contains(new Tuple<int, int>(index1 - 1, index2)) && !ignoreTargets.Contains(new Tuple<int, int>(index1 + 1, index2)))
                        {
                            if (ignoreX != -1 && num16 < 0 && (tile1.active) && (Main.tileSolid[(int)tile1.type] && !Main.tileSolidTop[(int)tile1.type]))
                            {
                                col = new Tuple<int, int>(index1 - 1, index2);
                                return true;
                            }
                            if (ignoreX != 1 && num16 > 0 && (tile2.active) && (Main.tileSolid[(int)tile2.type] && !Main.tileSolidTop[(int)tile2.type]))
                            {
                                col = new Tuple<int, int>(index1 + 1, index2);
                                return true;
                            }
                            if (tile3.active && (Main.tileSolid[(int)tile3.type] && !Main.tileSolidTop[(int)tile3.type]))
                            {
                                col = new Tuple<int, int>(index1, index2);
                                return true;
                            }
                        }
                        if (num14 == 0 && num15 == 0)
                        {
                            flag1 = true;
                            break;
                        }
                        index2 += num17;
                        --num15;
                        if (num14 == 0 && num15 == 0 && num19 == 1)
                            flag2 = true;
                    }
                    if (num14 != 0)
                    {
                        num13 = 2;
                        break;
                    }
                    break;
                case 2:
                    float num20 = num11 + num9;
                    int num21 = (int)num20;
                    num11 = num20 % 1f;
                    for (int index3 = 0; index3 < num21; ++index3)
                    {
                        if (Main.tile[index1, index2 - 1] == null)
                        {
                            col = new Tuple<int, int>(index1, index2 - 1);
                            return false;
                        }
                        if (Main.tile[index1, index2 + 1] == null)
                        {
                            col = new Tuple<int, int>(index1, index2 + 1);
                            return false;
                        }
                        Tile tile1 = Main.tile[index1, index2 - 1];
                        Tile tile2 = Main.tile[index1, index2 + 1];
                        Tile tile3 = Main.tile[index1, index2];
                        if (!ignoreTargets.Contains(new Tuple<int, int>(index1, index2)) && !ignoreTargets.Contains(new Tuple<int, int>(index1, index2 - 1)) && !ignoreTargets.Contains(new Tuple<int, int>(index1, index2 + 1)))
                        {
                            if (ignoreY != -1 && num17 < 0 && (tile1.active) && (Main.tileSolid[(int)tile1.type] && !Main.tileSolidTop[(int)tile1.type]))
                            {
                                col = new Tuple<int, int>(index1, index2 - 1);
                                return true;
                            }
                            if (ignoreY != 1 && num17 > 0 && (tile2.active) && (Main.tileSolid[(int)tile2.type] && !Main.tileSolidTop[(int)tile2.type]))
                            {
                                col = new Tuple<int, int>(index1, index2 + 1);
                                return true;
                            }
                            if (tile3.active && (Main.tileSolid[(int)tile3.type] && !Main.tileSolidTop[(int)tile3.type]))
                            {
                                col = new Tuple<int, int>(index1, index2);
                                return true;
                            }
                        }
                        if (num14 == 0 && num15 == 0)
                        {
                            flag1 = true;
                            break;
                        }
                        index1 += num16;
                        --num14;
                        if (num14 == 0 && num15 == 0 && num21 == 1)
                            flag2 = true;
                    }
                    if (num15 != 0)
                    {
                        num13 = 1;
                        break;
                    }
                    break;
            }
            if (Main.tile[index1, index2] == null)
            {
                col = new Tuple<int, int>(index1, index2);
                return false;
            }
            Tile tile = Main.tile[index1, index2];
            if (!ignoreTargets.Contains(new Tuple<int, int>(index1, index2)) && (tile.active && Main.tileSolid[(int)tile.type]) && !Main.tileSolidTop[(int)tile.type])
            {
                col = new Tuple<int, int>(index1, index2);
                return true;
            }
        }
        while (!(flag1 | flag2));
        col = new Tuple<int, int>(index1, index2);
        return true;
    }
    catch
    {
        col = new Tuple<int, int>(x1, y1);
        return false;
    }
}
#endregion

public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
{
    if (value.CompareTo(max) > 0)
        return max;
    return value.CompareTo(min) < 0 ? min : value;
}

public static Point ToTileCoordinates(Vector2 vec)
{
    return new Point((int)vec.X >> 4, (int)vec.Y >> 4);
}

public static bool InTileBounds(int x, int y, int lx, int ly, int hx, int hy)
{
    return x >= lx && x <= hx && (y >= ly && y <= hy);
}

#endregion

private class SmartCursorUsageInfo
{
    public Player player;
    public Item item;
    public Vector2 mouse;
    public Vector2 position;
    public Vector2 Center;
    public int screenTargetX;
    public int screenTargetY;
    public int reachableStartX;
    public int reachableEndX;
    public int reachableStartY;
    public int reachableEndY;
}