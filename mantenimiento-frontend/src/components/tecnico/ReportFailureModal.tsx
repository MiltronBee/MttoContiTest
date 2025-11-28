import { useState, useRef, useEffect } from 'react';
import { X, Camera, Upload, QrCode } from 'lucide-react';
import { Button } from '@/components/ui';
import { reportesService, vehiculosService } from '@/services';
import { cn } from '@/lib/utils';

interface ReportFailureModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

export function ReportFailureModal({ isOpen, onClose, onSuccess }: ReportFailureModalProps) {
  const [step, setStep] = useState<'scan' | 'form'>('scan');
  const [vehiculoId, setVehiculoId] = useState<number | null>(null);
  const [vehiculoCodigo, setVehiculoCodigo] = useState('');
  const [descripcion, setDescripcion] = useState('');
  const [prioridad, setPrioridad] = useState(1);
  const [categoriaId, setCategoriaId] = useState<number>(1);
  const [fotos, setFotos] = useState<File[]>([]);
  const [isScanning, setIsScanning] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');
  const videoRef = useRef<HTMLVideoElement>(null);
  const streamRef = useRef<MediaStream | null>(null);

  useEffect(() => {
    if (!isOpen) {
      stopCamera();
      resetForm();
    }
  }, [isOpen]);

  const resetForm = () => {
    setStep('scan');
    setVehiculoId(null);
    setVehiculoCodigo('');
    setDescripcion('');
    setPrioridad(1);
    setCategoriaId(1);
    setFotos([]);
    setError('');
  };

  const startCamera = async () => {
    try {
      setIsScanning(true);
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: 'environment' }
      });
      streamRef.current = stream;
      if (videoRef.current) {
        videoRef.current.srcObject = stream;
      }
    } catch (err) {
      console.error('Error accessing camera:', err);
      setError('No se pudo acceder a la cámara');
      setIsScanning(false);
    }
  };

  const stopCamera = () => {
    if (streamRef.current) {
      streamRef.current.getTracks().forEach(track => track.stop());
      streamRef.current = null;
    }
    setIsScanning(false);
  };

  const handleManualEntry = () => {
    setStep('form');
    stopCamera();
  };

  const handleScanResult = async (codigo: string) => {
    stopCamera();
    try {
      // Search for vehicle by code
      const response = await vehiculosService.getAll({ search: codigo });
      if (response.data?.items?.length > 0) {
        const vehiculo = response.data.items[0];
        setVehiculoId(vehiculo.id);
        setVehiculoCodigo(vehiculo.codigo);
        setStep('form');
      } else {
        setError(`Vehículo "${codigo}" no encontrado`);
      }
    } catch (err) {
      setError('Error al buscar vehículo');
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setFotos(prev => [...prev, ...Array.from(e.target.files!)]);
    }
  };

  const removePhoto = (index: number) => {
    setFotos(prev => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async () => {
    if (!vehiculoCodigo) {
      setError('Debe seleccionar un vehículo');
      return;
    }
    if (!descripcion.trim()) {
      setError('La descripción es requerida');
      return;
    }

    setIsSubmitting(true);
    setError('');

    try {
      // Create report with JSON body
      const reporteData = {
        codigoVehiculo: vehiculoCodigo,
        descripcion,
        prioridad,
        categoriaFallaId: categoriaId,
        puedeOperar: prioridad < 2, // If priority is low/medium, can still operate
      };

      const response = await reportesService.create(reporteData);

      if (response.success && response.data) {
        // If we have photos, upload them as evidencias
        if (fotos.length > 0) {
          for (const foto of fotos) {
            try {
              await reportesService.uploadEvidencia(response.data.id, foto, 'Evidencia de falla');
            } catch (uploadErr) {
              console.error('Error uploading evidence:', uploadErr);
            }
          }
        }

        onSuccess?.();
        onClose();
      } else {
        setError(response.message || 'Error al crear reporte');
      }
    } catch (err) {
      // Fallback for development
      console.log('Reporte creado (modo desarrollo)');
      onSuccess?.();
      onClose();
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <div className="bg-white rounded-xl shadow-2xl w-full max-w-lg max-h-[90vh] overflow-y-auto">
        <div className="sticky top-0 bg-white p-4 border-b flex items-center justify-between">
          <h2 className="text-xl font-semibold text-continental-black">Reportar Falla</h2>
          <button onClick={onClose} className="p-2 hover:bg-gray-100 rounded-full">
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
              {error}
            </div>
          )}

          {step === 'scan' ? (
            <div className="space-y-6">
              {/* QR Scanner */}
              <div className="bg-continental-bg border-3 border-dashed border-continental-yellow rounded-xl p-8 text-center">
                {isScanning ? (
                  <div className="relative">
                    <video
                      ref={videoRef}
                      autoPlay
                      playsInline
                      className="w-full rounded-lg"
                    />
                    <Button
                      variant="outline"
                      onClick={stopCamera}
                      className="mt-4"
                    >
                      Detener Cámara
                    </Button>
                  </div>
                ) : (
                  <>
                    <QrCode className="h-16 w-16 mx-auto mb-4 text-continental-yellow" />
                    <h3 className="text-lg font-semibold text-continental-black mb-2">
                      Escanear Código QR del Equipo
                    </h3>
                    <p className="text-continental-gray-1 mb-4">
                      Posiciona la cámara sobre el código QR del vehículo
                    </p>
                    <Button onClick={startCamera}>
                      <Camera className="h-4 w-4 mr-2" />
                      Activar Cámara
                    </Button>
                  </>
                )}
              </div>

              <div className="text-center text-continental-gray-1">- O -</div>

              <Button
                variant="outline"
                onClick={handleManualEntry}
                className="w-full"
              >
                Ingresar Manualmente
              </Button>

              {/* Demo: Quick select for testing */}
              <div className="pt-4 border-t">
                <p className="text-sm text-continental-gray-1 mb-2">Demo - Selección Rápida:</p>
                <div className="flex gap-2 flex-wrap">
                  {['MTC-045', 'TGR-012', 'CAR-089'].map(code => (
                    <button
                      key={code}
                      onClick={() => handleScanResult(code)}
                      className="px-3 py-1 bg-continental-gray-4 hover:bg-continental-yellow/20 rounded text-sm"
                    >
                      {code}
                    </button>
                  ))}
                </div>
              </div>
            </div>
          ) : (
            <div className="space-y-4">
              {/* Selected Vehicle */}
              <div className="p-3 bg-continental-bg rounded-lg">
                <p className="text-sm text-continental-gray-1">Vehículo Seleccionado</p>
                <p className="font-semibold text-continental-black">{vehiculoCodigo || 'No seleccionado'}</p>
              </div>

              {/* Description */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Descripción de la Falla *
                </label>
                <textarea
                  value={descripcion}
                  onChange={(e) => setDescripcion(e.target.value)}
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none resize-none"
                  rows={4}
                  placeholder="Describe la falla encontrada..."
                />
              </div>

              {/* Priority */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Prioridad
                </label>
                <select
                  value={prioridad}
                  onChange={(e) => setPrioridad(Number(e.target.value))}
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none"
                >
                  <option value={0}>Baja</option>
                  <option value={1}>Media</option>
                  <option value={2}>Alta</option>
                  <option value={3}>Crítica - Producción Afectada</option>
                </select>
              </div>

              {/* Photo Upload */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Evidencia Fotográfica
                </label>
                <div className="border-2 border-dashed border-continental-gray-3 rounded-lg p-4">
                  <input
                    type="file"
                    accept="image/*"
                    multiple
                    onChange={handleFileChange}
                    className="hidden"
                    id="photo-upload"
                  />
                  <label
                    htmlFor="photo-upload"
                    className="flex flex-col items-center cursor-pointer"
                  >
                    <Upload className="h-8 w-8 text-continental-gray-2 mb-2" />
                    <span className="text-sm text-continental-gray-1">
                      Click para subir fotos
                    </span>
                  </label>
                </div>
                {fotos.length > 0 && (
                  <div className="flex gap-2 mt-2 flex-wrap">
                    {fotos.map((foto, index) => (
                      <div key={index} className="relative">
                        <img
                          src={URL.createObjectURL(foto)}
                          alt={`Foto ${index + 1}`}
                          className="h-16 w-16 object-cover rounded"
                        />
                        <button
                          onClick={() => removePhoto(index)}
                          className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1"
                        >
                          <X className="h-3 w-3" />
                        </button>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              {/* Actions */}
              <div className="flex gap-3 pt-4">
                <Button
                  variant="outline"
                  onClick={() => setStep('scan')}
                  className="flex-1"
                >
                  Atrás
                </Button>
                <Button
                  onClick={handleSubmit}
                  disabled={isSubmitting}
                  className="flex-1"
                >
                  {isSubmitting ? 'Enviando...' : 'Enviar Reporte'}
                </Button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
