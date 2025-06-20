# Capital Architect - Testing Guide

## 🎮 **HOW TO TEST YOUR GAME**

### **Method 1: Unity Editor Testing (Recommended)**

1. **Open Unity Hub**
   - Launch Unity Hub
   - Open your project: `D:\lol\projects\game`

2. **Open the Main Scene**
   - In Unity, go to `Assets/Scenes/`
   - Open `Main.unity` or `SampleScene.unity`
   - Make sure all scripts are attached to GameObjects

3. **Set Up the Scene**
   - Create an empty GameObject called "GameManager"
   - Attach the `GameManager.cs` script to it
   - Create an empty GameObject called "SystemManager"
   - Attach the `SystemManager.cs` script to it
   - Create an empty GameObject called "ErrorHandler"
   - Attach the `ErrorHandler.cs` script to it

4. **Add UI Elements**
   - Create a Canvas in your scene
   - Add UI elements for the dashboard
   - Attach `UIDashboard.cs` to a GameObject in the Canvas

5. **Press Play**
   - Click the Play button in Unity
   - Watch the Console for system initialization messages
   - Check that all systems are loading properly

### **Method 2: Automated Testing**

The game includes built-in testing systems:

1. **System Integration Test**
   - The `SystemIntegrationTest.cs` will automatically run when you start the game
   - Look for test results in the Console window
   - Press 'T' key to run tests manually

2. **Final Verification**
   - The `FinalVerification.cs` provides comprehensive testing
   - Shows results in both Console and on-screen GUI
   - Automatically runs after 3 seconds

3. **Error Handler Testing**
   - The `ErrorHandler.cs` continuously monitors for issues
   - Logs all errors and warnings
   - Shows error UI if problems are detected

### **Method 3: Manual Feature Testing**

#### **Core Business Features**
1. **Business Creation**
   - Use `BusinessSetupUI.cs` to create a new business
   - Test different industries and regions
   - Verify initial capital is set correctly

2. **Employee Management**
   - Use `BusinessActionsUI.cs` to hire employees
   - Test employee training
   - Verify wage calculations

3. **Product Management**
   - Create products through the business system
   - Test production and sales
   - Verify inventory management

#### **Financial Systems**
1. **Finance UI Testing**
   - Open the Finance tab
   - Apply for loans
   - Make investments
   - Check financial metrics

2. **Logistics Testing**
   - Open the Logistics tab
   - Create warehouses
   - Place supply orders
   - Monitor inventory

#### **Advanced Systems**
1. **R&D System**
   - Open the R&D tab
   - Start research projects
   - Hire researchers
   - Monitor progress

2. **Marketing System**
   - Launch marketing campaigns
   - Monitor brand reputation
   - Check customer segments

3. **IPO System**
   - Check IPO requirements
   - Go public when ready
   - Monitor stock price

### **Method 4: Console Testing**

Open the Unity Console (Window > General > Console) and look for:

```
=== STARTING SYSTEM INTEGRATION TESTS ===
✓ GameManager found and accessible
✓ BusinessManager found and accessible
✓ MarketManager found and accessible
✓ FinanceSystem found and accessible
...
=== OVERALL RESULT: ALL TESTS PASSED ===
```

### **Method 5: Performance Testing**

1. **Frame Rate Monitoring**
   - The game includes FPS monitoring
   - Should maintain 30+ FPS
   - Check for performance warnings

2. **Memory Usage**
   - Monitor memory consumption
   - Should stay under 100MB
   - Watch for memory leaks

3. **System Load**
   - All 18 systems should load quickly
   - No long initialization delays
   - Smooth gameplay experience

## 🔧 **TROUBLESHOOTING**

### **Common Issues and Solutions**

#### **Issue: "System not found" errors**
**Solution**: Make sure all system scripts are attached to GameObjects in the scene

#### **Issue: UI elements not showing**
**Solution**: Check that UI components are properly assigned in the Inspector

#### **Issue: Game not starting**
**Solution**: Verify GameManager is in the scene and has all required references

#### **Issue: Null reference exceptions**
**Solution**: The ErrorHandler will catch these - check the Console for details

#### **Issue: Performance problems**
**Solution**: Check the FinalVerification results for performance warnings

## 📊 **WHAT TO LOOK FOR**

### **Successful Test Indicators**

✅ **Console Messages**
- "All systems initialized successfully!"
- "All systems found and connected!"
- "System integration test completed!"
- "Capital Architect is ready for production!"

✅ **UI Elements**
- Dashboard showing KPIs
- Tabs working properly
- Notifications appearing
- Error messages (if any) being handled gracefully

✅ **Gameplay**
- Business creation working
- Employee hiring functional
- Product production and sales
- Financial calculations accurate
- Market simulation running

✅ **Advanced Features**
- Research projects available
- Marketing campaigns working
- IPO system functional
- Crisis management active
- International expansion available

## 🎯 **QUICK START TESTING**

1. **Open Unity and your project**
2. **Open Main scene**
3. **Add required GameObjects with scripts**
4. **Press Play**
5. **Watch Console for test results**
6. **Try creating a business**
7. **Test basic operations**
8. **Check advanced systems**

## 🏆 **SUCCESS CRITERIA**

Your game is working correctly if:

- ✅ All 35 scripts compile without errors
- ✅ All 18 systems initialize successfully
- ✅ UI elements display properly
- ✅ Business operations work correctly
- ✅ Financial calculations are accurate
- ✅ Advanced systems are accessible
- ✅ No critical errors in Console
- ✅ Performance is smooth (30+ FPS)
- ✅ Memory usage is reasonable (<100MB)

## 🚀 **READY TO PLAY!**

Once all tests pass, your Capital Architect game is ready for players to enjoy the complete business simulation experience! 