package de.microsensys.sampleapp_java.getsensor;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.EventQueue;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.WindowEvent;
import java.awt.event.WindowListener;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Enumeration;
import java.util.Locale;

import javax.swing.AbstractButton;
import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.SpringLayout;
import javax.swing.SwingUtilities;
import javax.swing.border.EmptyBorder;
import javax.swing.text.DefaultCaret;

import de.microsensys.TELID.TELIDSensorInfo;
import de.microsensys.exceptions.MssException;
import de.microsensys.functions.RFIDFunctions_3000;
import de.microsensys.utils.ReaderIDInfo;

public class MainWindow extends JFrame implements ActionListener {

	private static final long serialVersionUID = 2542814797705018678L;
	
	private final String actionConnect = "connect";
	private final String actionDisconnect = "disconnect";
	private final String actionStartScan = "startScan";
	private final String actionStopScan = "stopScan";
	private final String actionAutoScan = "autoScan";
	private final String actionManualScan = "manualScan";
	
	private JPanel contentPane;
	
	private JComboBox comboBoxDevices;
	private JButton buttonConnect;
	private JButton buttonDisconnect;
	private ButtonGroup radioGroupScanMode;
	private JRadioButton radioAuto;
	private JRadioButton radioManual;
	private JTextField textFieldPhSize;
	private JLabel labelLibVersion;
	private JLabel labelReaderColor;
	private JLabel labelReaderInfo;
	private JButton buttonStart;
	private JButton buttonStop;
	private JLabel labelScanColor;
	private JLabel labelSerNo;
	private JLabel labelTelidType;
	private JLabel labelLastTimestamp;
	private JTextArea textAreaLogging;
	
	private CheckConnectingReader mCheckThread;
	private ScanThread mScanThread;
	
	RFIDFunctions_3000 mRFIDFunctions;

	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				try {
					MainWindow frame = new MainWindow();
					frame.addWindowListener(new WindowListener() {

						@Override
						public void windowOpened(WindowEvent e) {
							// TODO Auto-generated method stub
							
						}

						@Override
						public void windowClosing(WindowEvent e) {
							frame.closeCommunication();
						}

						@Override
						public void windowClosed(WindowEvent e) {
							// TODO Auto-generated method stub
							
						}

						@Override
						public void windowIconified(WindowEvent e) {
							// TODO Auto-generated method stub
							
						}

						@Override
						public void windowDeiconified(WindowEvent e) {
							// TODO Auto-generated method stub
							
						}

						@Override
						public void windowActivated(WindowEvent e) {
							// TODO Auto-generated method stub
							
						}

						@Override
						public void windowDeactivated(WindowEvent e) {
							// TODO Auto-generated method stub
							
						}
						
					});
					frame.setVisible(true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	/**
	 * Create the frame.
	 */
	public MainWindow() {
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setBounds(100, 100, 450, 600);
		
		SpringLayout layout;
		
		layout = new SpringLayout();
		contentPane = new JPanel();
		contentPane.setBorder(new EmptyBorder(5, 5, 5, 5));
		contentPane.setLayout(layout);
		setContentPane(contentPane);
		
		//TODO get available COM-Ports. Example for Windows, Linux and macOS
		String comPortNames[] = {"COM12", "/dev/ttyUSB0", "/dev/tty.usbserial-00000001"};
		
		ActionListener rbActionListener = new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				switch(e.getActionCommand()) {
				case actionAutoScan:
					textFieldPhSize.setEnabled(false);
					break;
				case actionManualScan:
					textFieldPhSize.setEnabled(true);
					break;
				}
			}
		};
		
		//Initialize UI variables
		comboBoxDevices = new JComboBox(comPortNames);
		buttonConnect = new JButton("CONNECT");
		buttonConnect.setActionCommand(actionConnect);
		buttonConnect.addActionListener(this);
		buttonDisconnect = new JButton("DISCONNECT");
		buttonDisconnect.setActionCommand(actionDisconnect);
		buttonDisconnect.addActionListener(this);
		buttonDisconnect.setEnabled(false);
		radioAuto = new JRadioButton("Auto", true);
		radioAuto.setActionCommand(actionAutoScan);
		radioAuto.addActionListener(rbActionListener);
		radioManual = new JRadioButton("Manual selection");
		radioManual.setActionCommand(actionManualScan);
		radioManual.addActionListener(rbActionListener);
		radioGroupScanMode = new ButtonGroup();
		radioGroupScanMode.add(radioAuto);
		radioGroupScanMode.add(radioManual);
		JLabel phSizeTitle = new JLabel("Physical size value:");
		textFieldPhSize = new JTextField();
		textFieldPhSize.setPreferredSize(new Dimension(100, 20));
		textFieldPhSize.setEnabled(false);
		textFieldPhSize.setText("04");
		phSizeTitle.setLabelFor(textFieldPhSize);
		labelLibVersion = new JLabel("V 6.0");
		labelReaderColor = new JLabel();
		labelReaderColor.setOpaque(true);
		labelReaderColor.setPreferredSize(new Dimension(10,10));
		labelReaderInfo = new JLabel("ReaderID: --");
		buttonStart = new JButton("START");
		buttonStart.setActionCommand(actionStartScan);
		buttonStart.addActionListener(this);
		buttonStart.setEnabled(false);
		buttonStop = new JButton("STOP");
		buttonStop.setActionCommand(actionStopScan);
		buttonStop.addActionListener(this);
		buttonStop.setEnabled(false);
		JLabel labelScanColorTitle = new JLabel("Last result: ");
		labelScanColor = new JLabel();
		labelScanColor.setOpaque(true);
		labelScanColor.setPreferredSize(new Dimension(10,10));
		JLabel labelSerNoTitle = new JLabel("Serial number: ");
		labelSerNo = new JLabel();
		JLabel labelTypeTitle = new JLabel("Type: ");
		labelTelidType = new JLabel();
		JLabel labelLastTimestampTitle = new JLabel("Last time read: ");
		labelLastTimestamp = new JLabel();
		JLabel labelValuesTitle = new JLabel("Sensor values:");
		textAreaLogging = new JTextArea();
		JScrollPane scrollPane = new JScrollPane(textAreaLogging);
		scrollPane.setVerticalScrollBarPolicy(JScrollPane.VERTICAL_SCROLLBAR_ALWAYS);
		DefaultCaret caret = (DefaultCaret)textAreaLogging.getCaret();
		caret.setUpdatePolicy(DefaultCaret.ALWAYS_UPDATE);
		textAreaLogging.setEditable(false);
		textAreaLogging.setLineWrap(true);
		textAreaLogging.setWrapStyleWord(true);
		
		//Add controls to panel
		contentPane.add(comboBoxDevices);
		layout.putConstraint(SpringLayout.NORTH, comboBoxDevices, 5, SpringLayout.NORTH, contentPane);
		layout.putConstraint(SpringLayout.WEST, comboBoxDevices, 5, SpringLayout.WEST, contentPane);
		layout.putConstraint(SpringLayout.EAST, comboBoxDevices, -5, SpringLayout.EAST, contentPane);
		
		JPanel topPanel = new JPanel();
		topPanel.setLayout(new GridLayout(0,2));
		topPanel.add(buttonConnect);
		topPanel.add(buttonDisconnect);
		contentPane.add(topPanel);
		layout.putConstraint(SpringLayout.NORTH, topPanel, 5, SpringLayout.SOUTH, comboBoxDevices);
		layout.putConstraint(SpringLayout.WEST, topPanel, 5, SpringLayout.WEST, contentPane);
		layout.putConstraint(SpringLayout.EAST, topPanel, -5, SpringLayout.EAST, contentPane);
		
		JPanel top2Panel = new JPanel();
		top2Panel.setLayout(new GridLayout(0,2));
		top2Panel.add(radioAuto);
		top2Panel.add(radioManual);
		contentPane.add(top2Panel);
		layout.putConstraint(SpringLayout.NORTH, top2Panel, 5, SpringLayout.SOUTH, topPanel);
		layout.putConstraint(SpringLayout.WEST, top2Panel, 5, SpringLayout.WEST, contentPane);
		layout.putConstraint(SpringLayout.EAST, top2Panel, -5, SpringLayout.EAST, contentPane);
		
		contentPane.add(phSizeTitle);
		layout.putConstraint(SpringLayout.NORTH, phSizeTitle, 5, SpringLayout.SOUTH, top2Panel);
		layout.putConstraint(SpringLayout.WEST, phSizeTitle, 20, SpringLayout.WEST, contentPane);
		contentPane.add(textFieldPhSize);
		layout.putConstraint(SpringLayout.NORTH, textFieldPhSize, 0, SpringLayout.NORTH, phSizeTitle);
		layout.putConstraint(SpringLayout.WEST, textFieldPhSize, 5, SpringLayout.EAST, phSizeTitle);
		
		JPanel top3Panel = new JPanel();
		top3Panel.setLayout(new GridLayout(0,3));
		top3Panel.add(labelLibVersion);
		top3Panel.add(labelReaderColor);
		top3Panel.add(labelReaderInfo);
		contentPane.add(top3Panel);
		layout.putConstraint(SpringLayout.NORTH, top3Panel, 5, SpringLayout.SOUTH, textFieldPhSize);
		layout.putConstraint(SpringLayout.WEST, top3Panel, 5, SpringLayout.WEST, contentPane);
		layout.putConstraint(SpringLayout.EAST, top3Panel, -5, SpringLayout.EAST, contentPane);
		
		contentPane.add(buttonStop);
		layout.putConstraint(SpringLayout.NORTH, buttonStop, 5, SpringLayout.SOUTH, top3Panel);
		layout.putConstraint(SpringLayout.EAST, buttonStop, -5, SpringLayout.EAST, contentPane);
		contentPane.add(buttonStart);
		layout.putConstraint(SpringLayout.NORTH, buttonStart, 0, SpringLayout.NORTH, buttonStop);
		layout.putConstraint(SpringLayout.WEST, buttonStart, 5, SpringLayout.WEST, contentPane);
		layout.putConstraint(SpringLayout.EAST, buttonStart, -5, SpringLayout.WEST, buttonStop);
		

		contentPane.add(labelScanColorTitle);
		layout.putConstraint(SpringLayout.NORTH, labelScanColorTitle, 5, SpringLayout.SOUTH, buttonStart);
		layout.putConstraint(SpringLayout.WEST, labelScanColorTitle, 20, SpringLayout.WEST, contentPane);
		contentPane.add(labelScanColor);
		layout.putConstraint(SpringLayout.NORTH, labelScanColor, 0, SpringLayout.NORTH, labelScanColorTitle);
		layout.putConstraint(SpringLayout.WEST, labelScanColor, 5, SpringLayout.EAST, labelScanColorTitle);
		
		contentPane.add(labelSerNoTitle);
		layout.putConstraint(SpringLayout.NORTH, labelSerNoTitle, 5, SpringLayout.SOUTH, labelScanColorTitle);
		layout.putConstraint(SpringLayout.EAST, labelSerNoTitle, 50, SpringLayout.EAST, labelScanColorTitle);
		contentPane.add(labelSerNo);
		layout.putConstraint(SpringLayout.NORTH, labelSerNo, 0, SpringLayout.NORTH, labelSerNoTitle);
		layout.putConstraint(SpringLayout.WEST, labelSerNo, 5, SpringLayout.EAST, labelSerNoTitle);
		
		contentPane.add(labelTypeTitle);
		layout.putConstraint(SpringLayout.NORTH, labelTypeTitle, 5, SpringLayout.SOUTH, labelSerNoTitle);
		layout.putConstraint(SpringLayout.WEST, labelTypeTitle, 0, SpringLayout.WEST, labelSerNoTitle);
		contentPane.add(labelTelidType);
		layout.putConstraint(SpringLayout.NORTH, labelTelidType, 0, SpringLayout.NORTH, labelTypeTitle);
		layout.putConstraint(SpringLayout.WEST, labelTelidType, 5, SpringLayout.EAST, labelTypeTitle);
		
		contentPane.add(labelLastTimestampTitle);
		layout.putConstraint(SpringLayout.NORTH, labelLastTimestampTitle, 5, SpringLayout.SOUTH, labelTypeTitle);
		layout.putConstraint(SpringLayout.WEST, labelLastTimestampTitle, 0, SpringLayout.WEST, labelTypeTitle);
		contentPane.add(labelLastTimestamp);
		layout.putConstraint(SpringLayout.NORTH, labelLastTimestamp, 0, SpringLayout.NORTH, labelLastTimestampTitle);
		layout.putConstraint(SpringLayout.WEST, labelLastTimestamp, 5, SpringLayout.EAST, labelLastTimestampTitle);
		
		contentPane.add(labelValuesTitle);
		layout.putConstraint(SpringLayout.NORTH, labelValuesTitle, 5, SpringLayout.SOUTH, labelLastTimestampTitle);
		layout.putConstraint(SpringLayout.WEST, labelValuesTitle, 0, SpringLayout.WEST, labelLastTimestampTitle);

		contentPane.add(scrollPane);
		layout.putConstraint(SpringLayout.NORTH, scrollPane, 5, SpringLayout.SOUTH, labelValuesTitle);
		layout.putConstraint(SpringLayout.WEST, scrollPane, 0, SpringLayout.WEST, labelValuesTitle);
		layout.putConstraint(SpringLayout.EAST, scrollPane, -5, SpringLayout.EAST, contentPane);
		layout.putConstraint(SpringLayout.SOUTH, scrollPane, -5, SpringLayout.SOUTH, contentPane);
	}
	
	protected void closeCommunication() {
		if (mRFIDFunctions != null){
			mRFIDFunctions.terminate();
        }
	}
	
	void setEnabledRadioButtons(ButtonGroup _bg, boolean _enabled) {
		Enumeration<AbstractButton> en = _bg.getElements();
		while(en.hasMoreElements()) {
			((JRadioButton)en.nextElement()).setEnabled(_enabled);
		}
	}

	public void appendResultText(String _toAppend) {
		appendResultText(_toAppend, true);
	}
	public void appendResultText(String _toAppend, boolean _autoAppendNewLine) {
		SwingUtilities.invokeLater(new Runnable() {
        	public void run() {
        		if (_autoAppendNewLine)
        			textAreaLogging.append(_toAppend + "\n");
        		else
        			textAreaLogging.append(_toAppend);
        	}
        });
	}
	
	@Override
	public void actionPerformed(ActionEvent e) {
		switch(e.getActionCommand()) {
		case actionConnect:
			connect();
			break;
		case actionDisconnect:
			disconnect();
			break;
		case actionStartScan:
			startScan();
			break;
		case actionStopScan:
			stopScan();
			break;
		}
	}
	
	private void connect() {
		textAreaLogging.setText("");
		
		//Before opening a new communication port, make sure that previous instance is disposed
		disposeRfidFunctions();
		if (comboBoxDevices.getSelectedIndex() == -1) {
			textAreaLogging.append("Please select a COM-Port to connect to");
			return;
		}
		comboBoxDevices.setEnabled(false);
		
		//Initialize SpcInterfaceControl instance.
		//	PortType = 2 --> Bluetooth
		//	PortName = selected COM-Port in ComboBox
		mRFIDFunctions = new RFIDFunctions_3000(1);
		mRFIDFunctions.setPortName(comboBoxDevices.getSelectedItem().toString());
		
		//Try to open communication port. This call does not block!!
		try {
			mRFIDFunctions.initialize();
			//No exception --> Check for process in a separate thread
			textAreaLogging.append("Connecting...");
			startCheckConnectingThread();
			buttonConnect.setEnabled(false);
			buttonDisconnect.setEnabled(true);
		} catch (MssException e) {
			//Exception thrown by "initialize" if something was wrong
			e.printStackTrace();
			textAreaLogging.append("Error opening port.");
			comboBoxDevices.setEnabled(true);
		}
	}

	public void connectProcedureFinished(boolean _connected, int _readerID) {
		SwingUtilities.invokeLater(new Runnable() {
        	public void run() {
        		if (_connected) {
        			textAreaLogging.append("\nCONNECTED\n");
        			if (_readerID > 0) {
        				labelReaderInfo.setText("ReaderID: " + String.valueOf(_readerID));
        				labelReaderColor.setBackground(Color.GREEN);
        			}
        			else
        				labelReaderColor.setBackground(Color.YELLOW);
        		}
        		else {
        			textAreaLogging.append("\nReader NOT connected\n");
        		}
        		setUiConnected(_connected);
        	}
        });
	}
	public void sensorFound(TELIDSensorInfo _sensorInfo) {
		SwingUtilities.invokeLater(new Runnable() {
        	public void run() {
        		if (_sensorInfo != null) {
        			labelScanColor.setBackground(Color.GREEN);
        			labelSerNo.setText(_sensorInfo.getSerialNumber());
        			labelTelidType.setText(_sensorInfo.getTELIDDescription());
        			labelLastTimestamp.setText(new SimpleDateFormat("HH:mm:ss", Locale.getDefault()).format(new Date()));
        			textAreaLogging.setText("");
        			for(String sensorValue : _sensorInfo.getSensorValueStrings()) {
        				textAreaLogging.append(sensorValue + "\n");
        			}
        		}
        		else {
        			labelScanColor.setBackground(Color.RED);
        		}
        	}
        });
	}
	
	private void setUiConnected(boolean _connected) {
		SwingUtilities.invokeLater(new Runnable() {
        	public void run() {
        		if (_connected) {
        			buttonStart.setEnabled(true);
        			radioAuto.setEnabled(true);
        			radioManual.setEnabled(true);
        		}
        		else {
        			comboBoxDevices.setEnabled(true);
        			buttonConnect.setEnabled(true);
        			
        			buttonDisconnect.setEnabled(false);
        			buttonStart.setEnabled(false);
        			buttonStop.setEnabled(false);
        			labelReaderInfo.setText("");
        			labelReaderColor.setBackground(Color.WHITE);
        		}
        	}
        });
	}
	
	private void startScan() {
        disposeScanThread();
        if (radioAuto.isSelected())
            mScanThread = new ScanThread();
        else
            mScanThread = new ScanThread(Byte.valueOf(textFieldPhSize.getText().toString()));
        mScanThread.start();
        buttonStart.setEnabled(false);
        buttonStop.setEnabled(true);
    }
    private void stopScan() {
        disposeScanThread();
        buttonStart.setEnabled(true);
        buttonStop.setEnabled(false);
    }
	
	private void disconnect() {
		disposeCheckConnectingThread();
		disposeScanThread();
		disposeRfidFunctions();
		setUiConnected(false);
		textAreaLogging.append("\n DISCONNECTED");
	}
	private void disposeRfidFunctions(){
        if (mRFIDFunctions != null)
        	mRFIDFunctions.terminate();
        	mRFIDFunctions = null;
    }
	private void disposeCheckConnectingThread() {
		if (mCheckThread != null){
            mCheckThread.cancel();
        }
		mCheckThread = null;
	}
	private void disposeScanThread(){
        if (mScanThread != null){
            mScanThread.cancel();
        }
        mScanThread = null;
    }
	public void startCheckConnectingThread(){
        disposeCheckConnectingThread();
        mCheckThread = new CheckConnectingReader();
        mCheckThread.start();
    }
    private class CheckConnectingReader extends Thread {
        private boolean crt_loop;

        CheckConnectingReader(){
            crt_loop = true;
        }

        @Override
        public void run() {
            while (crt_loop){
                if (mRFIDFunctions.isConnecting()){
                    //Still trying to connect -> Wait and continue
                    try {
                        Thread.sleep(200);
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                    appendResultText(".", false);
                    continue;
                }
                //Connecting finished! Check if connected or not connected
                ReaderIDInfo readerId = null;
                if (mRFIDFunctions.isConnected()){
                    try {
                        readerId = mRFIDFunctions.readReaderID();
                        if (readerId != null) {
                            connectProcedureFinished(true, readerId.getReaderID());
                        }
                    }
                    catch (Exception ignore){}
                }
                if (readerId == null)
                    connectProcedureFinished(false, 0);

                //Stop this thread
                cancel();
            }
        }

        void cancel(){
            crt_loop = false;
        }
    }
    private class ScanThread extends Thread {
        private boolean st_loop;
        private boolean st_Auto = true;
        private byte st_phSize;

        ScanThread(){
            st_loop = true;
        }
        ScanThread(byte _phSize){
            st_loop = true;
            st_Auto = false;
            st_phSize = _phSize;
        }

        @Override
        public void run() {
            TELIDSensorInfo sensorInfo;

            while (st_loop){
                sensorInfo = null;
                if (st_Auto){
                    try {
                        sensorInfo = mRFIDFunctions.getSensorData(0xFC);
                    } catch (MssException e) {
                        e.printStackTrace();
                    }
                }
                else{
                    try {
                        sensorInfo = mRFIDFunctions.getSensorData(st_phSize);
                    } catch (MssException e) {
                        e.printStackTrace();
                    }
                }
                if (st_loop){
                    sensorFound(sensorInfo);
                }
                try {
                    Thread.sleep(1000);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }

        void cancel(){
            st_loop = false;
        }
    }
}
